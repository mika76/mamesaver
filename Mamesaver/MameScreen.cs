using System;
using System.Windows.Forms;
using Mamesaver.Models.Configuration;
using Mamesaver.Power;
using Mamesaver.Services.Windows;
using Serilog;
using LayoutSettings = Mamesaver.Models.Configuration.LayoutSettings;
using Timer = System.Timers.Timer;

namespace Mamesaver
{
    /// <summary>
    ///     A screen that will launch MAME with a random game from the list. If configured, this screen also handles hotkey events which
    ///     affect the MAME process.
    /// </summary>
    internal class MameScreen : BlankScreen
    {
        private readonly Settings _settings;
        private readonly GamePlayManager _gamePlayManager;
        private readonly IActivityHook _activityHook;
        private readonly PowerManager _powerManager;
        private readonly SplashScreen _splashSettings;

        /// <summary>
        ///     Timer for game execution.
        /// </summary>
        private Timer _gameTimer;

        /// <summary>
        ///     Timer for splash screen display.
        /// </summary>
        private Timer _splashTimer;

        private bool _initialised;

        public MameScreen(
            Settings settings,
            LayoutSettings layoutSettings,
            GamePlayManager gamePlayManager,
            IActivityHook activityHook,
            PowerManager powerManager) : base(layoutSettings, powerManager)
        {
            _settings = settings;
            _gamePlayManager = gamePlayManager;
            _activityHook = activityHook;
            _powerManager = powerManager;

            _splashSettings = layoutSettings.SplashScreen;
        }

        public override void Initialise(Screen screen)
        {
            base.Initialise(screen);

            _powerManager.SleepTriggered += OnSleep;
            _gamePlayManager.OnGameStarted += OnGameStarted;
            _gamePlayManager.OnPlayGame += OnPlayGame;
            _gamePlayManager.OnStartGame += OnStartGame;

            BackgroundForm.mameLogo.Visible = true;
            BackgroundForm.Load += OnFormBackground_Load;

            InitGameTimer();
            InitSplashTimer();

            // Start listening for mouse and keyboard activity
            _activityHook.Start();

            _initialised = true;

            Log.Information("Initialised primary MAME screen");
        }

        private void OnStartGame(object sender, EventArgs args)
        {
            try
            {
                _gameTimer.Stop();

                // Display splash screen for next game if required
                DisplaySplashText();
                _splashTimer.Start();

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error preparing next game");
            }
        }

        private void OnPlayGame(object sender, EventArgs args)
        {
            HideBackgroundForm();
            _gameTimer.Stop();
        }

        private void OnGameStarted(object sender, EventArgs args) => _gameTimer.Start();

        /// <summary>
        ///     Handles sleep events, stopping game timers.
        /// </summary>
        private void OnSleep(object sender, EventArgs args)
        {
            Log.Information("Stopping game timers due to a sleep event");

            _powerManager.SleepTriggered -= OnSleep;
            DisposeTimers();
            HideBackgroundForm();
        }

        private void InitSplashTimer()
        {
            var splashSeconds = _splashSettings.DurationSeconds;
            var splashInterval = (int) TimeSpan.FromSeconds(splashSeconds).TotalMilliseconds;

            // This is a bit of a bodge; set the splash screen duration to 1ms if the splash is disabled
            if (!_splashSettings.Enabled) splashInterval = 1;

            _splashTimer = new Timer { Interval = splashInterval };
            _splashTimer.Elapsed += SplashTimerTick;
        }

        private void InitGameTimer()
        {
            var minutes = _settings.MinutesPerGame;
            var gameTimerInterval = (int) TimeSpan.FromMinutes(minutes).TotalMilliseconds;

            _gameTimer = new Timer { Interval = gameTimerInterval };
            _gameTimer.Elapsed += GameTimerTick;
        }

        /// <summary>
        ///     Invoked when when the splash screen timer has fired, indicating that the game should start.
        /// </summary>
        private void SplashTimerTick(object sender, EventArgs e)
        {
            _splashTimer.Stop();
            _gamePlayManager.RunGame();
        }

        private void OnFormBackground_Load(object sender, EventArgs e)
        {
            _splashTimer.Start();
            DisplaySplashText();
        }

        /// <summary>
        ///     Displays the game description and details on the splash screen.
        /// </summary>
        private void DisplaySplashText()
        {
            if (!_splashSettings.Enabled) return;

            Log.Debug("Displaying splash screen");

            var game = _gamePlayManager.CurrentGame();
            BackgroundForm.SetGameText(game.Description, $@"{game.Year} {game.Manufacturer}");
        }

        /// <summary>
        ///     Invoked after a game has finished running.
        /// </summary>
        private void GameTimerTick(object sender, EventArgs e)
        {
            _gamePlayManager.NextGame();        
            _gamePlayManager.StartGame();
        }

        /// <summary>
        ///     Stop the timer, set cancelled flag, close any current process and close the background form.
        ///     Once this has all been done, the application should end.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || !_initialised) return;

            Log.Information("Closing primary MAME screen {screen}", Screen.DeviceName);

            try
            {
                DisposeTimers();
                HideBackgroundForm();
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

        ~MameScreen() => Dispose(false);
    }
}