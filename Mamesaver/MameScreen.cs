using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Mamesaver.Layout;
using Mamesaver.Windows;
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

        private Timer _gameTimer;
        private Timer _splashTimer;
        private bool _closed;

        private readonly object _syncLock = new object();

        private readonly Random _random = new Random();

        public Process GameProcess { get; set; }

        public MameScreen(
            Settings settings,
            LayoutSettings layoutSettings,
            GameList gameList,
            LayoutBuilder layoutBuilder,
            MameInvoker invoker,
            BackgroundForm backgroundForm) : base(backgroundForm)
        {
            _settings = settings;
            _layoutBuilder = layoutBuilder;
            _invoker = invoker;

            _splashSettings = layoutSettings.SplashScreen;
            _selectedGames = gameList.SelectedGames.OrderBy(_ => _random.Next()).ToList();
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

            var game = CurrentGame();
            BackgroundForm.SetGameText(game.Description, $@"{game.Year} {game.Manufacturer}");
        }

        /// <summary>
        ///     Invoked after a game has finished running.
        /// </summary>
        private void GameTimerTick(object sender, EventArgs e)
        {
            try
            {
                _gameTimer.Stop();

                // End the currently playing game
                if (GameProcess != null && !GameProcess.HasExited) GameProcess.CloseMainWindow();

                // Retrieve next game
                _gameIndex++;
                if (_gameIndex >= _selectedGames.Count) _gameIndex = 0;

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

                    if (GameProcess != null && !GameProcess.HasExited)
                    {
                        // Minimise and then exit. Minimising it makes it disappear instantly
                        if (GameProcess.MainWindowHandle != IntPtr.Zero)
                            WindowsInterop.MinimizeWindow(GameProcess.MainWindowHandle);

                        // Stop the MAME process. Note that we need to call Kill() instead of CloseMainWindow() in case 
                        // we are terminating MAME before the window has been created.
                        GameProcess.Kill();
                    }

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