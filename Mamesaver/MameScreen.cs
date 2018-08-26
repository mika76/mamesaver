using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Mamesaver.Layout;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver
{
    /// <summary>
    ///     A screen that will launch MAME with a random game from the list.
    /// </summary>
    internal class MameScreen : BlankScreen
    {
       private readonly Settings _settings;
        private readonly GameList _gameList;
        private readonly LayoutBuilder _layoutBuilder;
        private readonly MameInvoker _invoker;

        private Timer _timer;
        private bool _closed;
 
        private readonly object _syncLock = new object();

        public Process GameProcess { get; set; }

        public MameScreen(
            Settings settings,
            GameList gameList,
            LayoutBuilder layoutBuilder,
            MameInvoker invoker,
            BackgroundForm backgroundForm) : base(backgroundForm)
        {
            _settings = settings;
            _gameList = gameList;
            _layoutBuilder = layoutBuilder;
            _invoker = invoker;
        }

        public override void Initialise(Screen screen, Action onClosed)
        {
            base.Initialise(screen, onClosed);

            BackgroundForm.mameLogo.Visible = true;

            // Set up the timer
            var minutes = _settings.MinutesPerGame;
            _timer = new Timer { Interval = minutes * 60000 };
            _timer.Tick += timer_Tick;

            Log.Information("Initialised screen");

            BackgroundForm.Load += OnFormBackground_Load;
        }

        private void OnFormBackground_Load(object sender, EventArgs e)
        {
            try
            {
                // Start the first game
                GameProcess = RunRandomGame(_gameList.SelectedGames);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to start game");
                Close();

                throw;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();

                // End the currently playing game
                if (GameProcess != null && !GameProcess.HasExited) GameProcess.CloseMainWindow();

                // Start new game
                Log.Information("New game scheduled for execution");
                GameProcess = RunRandomGame(_gameList.SelectedGames);
           }
            catch (Exception ex)
            {
                Log.Error(ex, "MameScreen tick");
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
                    _timer?.Stop();
                    _timer = null;

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
        ///     Gets a random number and then runs <see cref="RunGame" /> using the game in the
        ///     <see cref="List{T}" />.
        /// </summary>
        /// <param name="gameList"></param>
        /// <returns>The <see cref="Process" /> running the game</returns>
        private Process RunRandomGame(List<Game> gameList)
        {
            // Get random game
            var r = new Random();
            var randomIndex = r.Next(0, gameList.Count - 1);
            var randomGame = gameList[randomIndex];
            return RunGame(randomGame);
        }

        /// <summary>
        ///     Runs the process.
        /// </summary>
        /// <param name="game"></param>
        /// <returns>The <see cref="Process" /> running the game</returns>
        private Process RunGame(Game game)
        {
            // Set the game name and details on the splash screen if requested
            var splashSettings = _settings.LayoutSettings.SplashScreen;
            if (splashSettings.Enabled)
            {
                BackgroundForm.SetGameText(game.Description, $@"{game.Year} {game.Manufacturer}");

                // Show the splash screen
                var end = DateTime.Now.AddSeconds(splashSettings.DurationSeconds);
                while (DateTime.Now < end)
                {
                    Application.DoEvents();
                }
            }
            else
            {
                Application.DoEvents();
            }

            // Don't attempt to run a game if the screen has been closed
            if (_closed) return null;

            Log.Information("Running game {description} {year} {manufacturer} on display {display}",
                game.Description,
                game.Year, game.Manufacturer, Screen.DeviceName);

            // Start the timer and the process
            _timer.Start();

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