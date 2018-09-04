using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mamesaver.Configuration;
using Mamesaver.Configuration.Models;
using Mamesaver.Hotkeys;
using Mamesaver.Layout;
using Mamesaver.Power;
using Serilog;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;
using Timer = System.Windows.Forms.Timer;

namespace Mamesaver
{
    /// <summary>
    ///     A screen that will launch MAME with a random game from the list. If configured, this screen also handles hotkey events which
    ///     affect the MAME process.
    /// </summary>
    internal class MameScreen : BlankScreen, IDisposable
    {
        private readonly Settings _settings;
        private readonly GameList _gameList;
        private readonly GameListStore _gameListStore;
        private readonly HotKeyManager _hotKeyManager;
        private readonly LayoutBuilder _layoutBuilder;
        private readonly PowerManager _powerManager;
        private readonly MameInvoker _invoker;
        private readonly SplashScreen _splashSettings;

        /// <summary>
        ///     Shuffled games that will be played.
        /// </summary>
        private List<Game> _selectedGames;

        /// <summary>
        ///     Index of game in <see cref="_selectedGames"/> which is being played.
        /// </summary>
        private int _gameIndex;

        /// <summary>
        ///     Timer for game execution.
        /// </summary>
        private Timer _gameTimer;

        /// <summary>
        ///     Timer for splash screen display.
        /// </summary>
        private Timer _splashTimer;

        private readonly Random _random = new Random();
        private bool _initialised;
        private Action _onClose;
        private CancellationToken _token;

        /// <summary>
        ///     MAME process running the current game, or <c>null</c> if not started.
        /// </summary>
        public Process GameProcess { get; set; }

        public MameScreen(
            Settings settings,
            LayoutSettings layoutSettings,
            GameList gameList,
            GameListStore gameListStore,
            HotKeyManager hotKeyManager,
            LayoutBuilder layoutBuilder,
            PowerManager powerManager,
            MameInvoker invoker) : base(layoutSettings, powerManager)
        {
            _settings = settings;
            _gameList = gameList;
            _gameListStore = gameListStore;
            _hotKeyManager = hotKeyManager;
            _layoutBuilder = layoutBuilder;
            _powerManager = powerManager;
            _invoker = invoker;

            _splashSettings = layoutSettings.SplashScreen;
        }

        public void Initialise(Screen screen, CancellationToken token, Action onClose)
        {
            base.Initialise(screen);
            _token = token;
            _onClose = onClose;

            _selectedGames = _gameList.SelectedGames.OrderBy(_ => _random.Next()).ToList();
            _hotKeyManager.HotKeyPressed += ProcessHotKey;
            _powerManager.SleepTriggered += OnSleep;

            BackgroundForm.mameLogo.Visible = true;
            BackgroundForm.Load += OnFormBackground_Load;

            InitGameTimer();
            InitSplashTimer();

            _initialised = true;

            Log.Information("Initialised primary MAME screen");
        }

        /// <summary>
        ///     Handles sleep events, closing MAME.
        /// </summary>
        private void OnSleep(object sender, EventArgs args)
        {
            Log.Information("Stopping MAME due to sleep event");

            _powerManager.SleepTriggered -= OnSleep;
            DisposeTimers();
            HideBackgroundForm();

            _invoker.Stop(GameProcess);
        }

        /// <summary>
        ///     Handle a hot key event.
        /// </summary>
        private void ProcessHotKey(object sender, HotKeyEventArgs e)
        {
            var currentGame = CurrentGame();

            switch (e.HotKey)
            {
                case HotKey.NextGame:
                    Log.Information("Skipping to next game");
                    NextGame();
                    StartGame();
                    break;

                case HotKey.PreviousGame:
                    Log.Information("Skipping to previous game");
                    PreviousGame();
                    StartGame();
                    break;

                case HotKey.DeselectGame:
                    // Sanity check that the user isn't attempting to deselect their last selected game
                    if (_selectedGames.Count == 1)
                    {
                        Log.Information("Attempting to deselect single remaining game; ignoring request");
                        return;
                    }

                    Log.Information("Deselecting {game}", currentGame.Name);

                    // Update game selection and update store
                    _gameListStore.ChangeSelection(currentGame.Name, false);
                    _selectedGames.Remove(currentGame);
                    if (_gameIndex >= _selectedGames.Count) _gameIndex = 0;

                    // Start next game
                    StartGame();
                    break;

                case HotKey.PlayGame:

                    // Unsubscribe from hotkey events as control is being handed over to MAME
                    _hotKeyManager.HotKeyPressed -= ProcessHotKey;

                   HideBackgroundForm();

                    _gameTimer.Stop();

                    // Close existing MAME instance running in screensaver
                    _invoker.Stop(GameProcess);

                    // Run MAME without screensaver options
                    _invoker.Run(currentGame.Name).WaitForExit(int.MaxValue);

                    // Close screensaver after game has terminated
                    _onClose();
                    break;
            }
        }

        private void InitSplashTimer()
        {
            var splashSeconds = _splashSettings.DurationSeconds;
            var splashInterval = (int) TimeSpan.FromSeconds(splashSeconds).TotalMilliseconds;

            // This is a bit of a bodge; set the splash screen duration to 1ms if the splash is disabled
            if (!_splashSettings.Enabled) splashInterval = 1;

            _splashTimer = new Timer { Interval = splashInterval };
            _splashTimer.Tick += SplashTimerTick;
        }

        private void InitGameTimer()
        {
            var minutes = _settings.MinutesPerGame;
            var gameTimerInterval = (int) TimeSpan.FromMinutes(minutes).TotalMilliseconds;

            _gameTimer = new Timer { Interval = gameTimerInterval };
            _gameTimer.Tick += GameTimerTick;
        }

        /// <summary>
        ///     Invoked when when the splash screen timer has fired, indicating that the game should start.
        /// </summary>
        private void SplashTimerTick(object sender, EventArgs e)
        {
            _splashTimer.Stop();

            try
            {
                // Start MAME
                GameProcess = RunGame();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to start game");
                _onClose();

                throw;
            }
        }

        private void OnFormBackground_Load(object sender, EventArgs e)
        {
            _splashTimer.Start();
            DisplaySplashText();
        }

        private Game CurrentGame() => _selectedGames[_gameIndex];

        /// <summary>
        ///     Displays the game description and details on the splash screen.
        /// </summary>
        private void DisplaySplashText()
        {
            if (!_splashSettings.Enabled) return;

            Log.Debug("Displaying splash screen");

            var game = CurrentGame();
            BackgroundForm.SetGameText(game.Description, $@"{game.Year} {game.Manufacturer}");
        }

        /// <summary>
        ///     Invoked after a game has finished running.
        /// </summary>
        private void GameTimerTick(object sender, EventArgs e)
        {
            NextGame();        
            StartGame();
        }

        /// <summary>
        ///     Executes MAME, running the currently selected game.
        /// </summary>
        private void StartGame()
        {
            try
            {
                _gameTimer.Stop();

                // End the currently playing game
                _invoker.Stop(GameProcess);

                // Display splash screen for next game if required
                DisplaySplashText();
                _splashTimer.Start();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error preparing next game");
            }
        }

        /// <summary>
        ///     Selects the next game for play.
        /// </summary>
        private void NextGame()
        {
            _gameIndex++;
            if (_gameIndex >= _selectedGames.Count) _gameIndex = 0;
        }

        /// <summary>
        ///     Selects the previous game for play.
        /// </summary>
        private void PreviousGame()
        {
            _gameIndex--;
            if (_gameIndex < 0) _gameIndex = _selectedGames.Count - 1;
        }

        /// <summary>
        ///     Stop the timer, set cancelled flag, close any current process and close the background form.
        ///     Once this has all been done, the application should end.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            if (!disposing || !_initialised) return;

            Log.Information("Closing primary MAME screen {screen}", Screen.DeviceName);

            try
            {
                DisposeTimers();
                HideBackgroundForm();

                // Stop MAME and wait for it to terminate
                _invoker.Stop(GameProcess);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error closing screen");
            }
        }

        private void DisposeTimers()
        {
            _gameTimer?.Stop();
            _splashTimer?.Stop();
            _gameTimer = _splashTimer = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MameScreen() => Dispose(false);

        /// <summary>
        ///     Runs the MAME game.
        /// </summary>
        /// <returns>The <see cref="Process" /> running the game</returns>
        private Process RunGame()
        {
            // Don't attempt to start MAME process if we are exiting
            if (_token.IsCancellationRequested) return null;

            var game = CurrentGame();

            Log.Information("Running game {description} {year} {manufacturer} on display {display}",
                game.Description,
                game.Year, game.Manufacturer, Screen.DeviceName);

            // Start the timer and the process
            _gameTimer.Start();

            // Create layout and run game
            var arguments = new List<string>
            {
                game.Name,
                _settings.CommandLineOptions,
                "-screen",
                $"\"{Screen.DeviceName}\""
            };

            // Enable in-game titles if required
            if (_settings.LayoutSettings.InGameTitles.Enabled)
            {
                var artPath = _layoutBuilder.EnsureLayout(game, Screen.Bounds.Width, Screen.Bounds.Height);
                arguments.Add("-artpath");
                arguments.Add(artPath);
            }

            return _invoker.Run(arguments.ToArray());
        }
    }
}