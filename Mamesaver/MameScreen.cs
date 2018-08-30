using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Mamesaver.Configuration;
using Mamesaver.Configuration.Models;
using Mamesaver.Layout;
using Serilog;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;

namespace Mamesaver
{
    /// <summary>
    ///     A screen that will launch MAME with a random game from the list.
    /// </summary>
    internal class MameScreen : BlankScreen
    {
        private readonly Settings _settings;
        private readonly GameListStore _gameListStore;
        private readonly LayoutBuilder _layoutBuilder;
        private readonly MameInvoker _invoker;
        private readonly SplashScreen _splashSettings;

        /// <summary>
        ///     Shuffled games that will be played.
        /// </summary>
        private readonly List<Game> _selectedGames;

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

        /// <summary>
        ///     If <see cref="Close" /> has been invoked, indicating that no further games should be played.
        /// </summary>
        private bool _closed;

        private readonly object _syncLock = new object();

        private readonly Random _random = new Random();

        /// <summary>
        ///     MAME process running the current game, or <c>null</c> if not started.
        /// </summary>
        public Process GameProcess { get; set; }

        public MameScreen(
            Settings settings,
            LayoutSettings layoutSettings,
            GameList gameList,
            GameListStore gameListStore,
            LayoutBuilder layoutBuilder,
            MameInvoker invoker,
            BackgroundForm backgroundForm) : base(backgroundForm, settings)
        {
            _settings = settings;
            _gameListStore = gameListStore;
            _layoutBuilder = layoutBuilder;
            _invoker = invoker;

            _splashSettings = layoutSettings.SplashScreen;

            _selectedGames = gameList.SelectedGames.OrderBy(_ => _random.Next()).ToList();
        }

        public override void actHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (_settings.HotKeys) ProcessHotKeys(e);
            else base.actHook_KeyDown(sender, e);
        }

        public override void Initialise(Screen screen, Action onClosed)
        {
            base.Initialise(screen, onClosed);

            BackgroundForm.mameLogo.Visible = true;
            BackgroundForm.Load += OnFormBackground_Load;

            InitGameTimer();
            InitSplashTimer();

            Log.Information("Initialised primary MAME screen");
        }

        /// <summary>
        ///     Handle a key down event, processing hotkey actions.
        /// </summary>
        private void ProcessHotKeys(KeyEventArgs e)
        {
            var currentGame = CurrentGame();

            switch (e.KeyCode)
            {
                // Play next game
                case Keys.Right:
                    Log.Information("Skipping to next game");
                    NextGame();
                    StartGame();
                    break;

                // Play previous game
                case Keys.Left:
                    Log.Information("Skipping to previous game");
                    PreviousGame();
                    StartGame();
                    break;

                // Remove current game from circulation and play next game
                case Keys.Delete:
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

                case Keys.Enter:

                    // Play game

                    // Hide the background form elements to provide a seamless transition. We are also
                    // forcing an immediate refresh to avoid any flicker of the MAME logo before it's hidden/
                    BackgroundForm.HideAll();
                    BackgroundForm.Refresh();

                    _gameTimer.Stop();
                    UnbindActivityHooks();

                    // Close existing MAME instance running in screensaver
                    _invoker.Kill(GameProcess);

                    // Run MAME without screensaver options
                    _invoker.Run(currentGame.Name).WaitForExit(int.MaxValue);

                    // Close screensaver after game has terminated
                    Close();
                    break;

                // Exit screensaver for unhandled keys
                default:
                    Close();
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
                // Start the first game
                GameProcess = RunGame();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to start game");
                Close();

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
        ///     Displays the game description and details on the splash screen/
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
                _invoker.Kill(GameProcess);

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
        public override void Close()
        {
            Log.Information("Closing primary MAME screen {screen}", Screen.DeviceName);
            lock (_syncLock)
            {
                try
                {
                    _gameTimer?.Stop();
                    _splashTimer?.Stop();
                    _gameTimer = _splashTimer = null;

                    // Stop MAME and wait for it to terminate
                    _invoker.Kill(GameProcess, true);

                    _closed = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error closing screen");
                }

                base.Close();
            }
        }

        /// <summary>
        ///     Runs the MAME game.
        /// </summary>
        /// <returns>The <see cref="Process" /> running the game</returns>
        private Process RunGame()
        {
            var game = CurrentGame();

            // Don't attempt to run a game if the screen has been closed
            if (_closed) return null;

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