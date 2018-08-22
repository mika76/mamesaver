using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Mamesaver.Layout;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver
{
    /// <summary>
    /// A screen that will launch Mame and a random game from the list
    /// </summary>
    public class MameScreen : BlankScreen
    {
        private readonly List<Game> _gameList;
        private readonly bool _runGame;
        private LayoutBuilder _layoutBuilder;

        Timer _timer;
        private readonly object _syncLock = new object();

        public MameScreen(Screen screen, List<Game> gameList, Action<BlankScreen> onClosed, bool runGame) : base(screen, onClosed)
        {
            _gameList = gameList;
            _runGame = runGame;
        }

        public Process GameProcess { get; set; }

        public override void Initialise()
        {
            base.Initialise();

            _layoutBuilder = new LayoutBuilder();

            // Set up the timer
            var minutes = Settings.Minutes;
            _timer = new Timer{ Interval = minutes * 60000 };
            _timer.Tick += timer_Tick;

            FrmBackground.Load += OnFormBackground_Load;
        }

        void OnFormBackground_Load(object sender, EventArgs e)
        {
            // Start the first game
            GameProcess = RunRandomGame(_gameList);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();

                // End the currently playing game
                if (GameProcess != null && !GameProcess.HasExited) GameProcess.CloseMainWindow();

                // Start new game
                GameProcess = RunRandomGame(_gameList);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "MameScreen tick");
            }
        }

        /// <summary>
        /// Stop the timer, set cancelled flag, close any current process and close the background form. 
        /// Once this has all been done, the application should end.
        /// </summary>
        public override void Close()
        {
            lock (_syncLock)
            {
                try
                {
                    _timer?.Stop();
                    _timer = null;

                    if (GameProcess != null && !GameProcess.HasExited)
                    {
                        // Minimise and then exit. Minimising it makes it disappear instantly
                        if (GameProcess.MainWindowHandle != IntPtr.Zero) WindowsInterop.MinimizeWindow(GameProcess.MainWindowHandle);
                        GameProcess.CloseMainWindow();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error closing screen");
                }

                base.Close();
            }
        }

        /// <summary>
        /// Gets a random number and then runs <see cref="RunGame"/> using the game in the
        /// <see cref="List{T}"/>.
        /// </summary>
        /// <param name="gameList"></param>
        /// <returns>The <see cref="Process"/> running the game</returns>
        private Process RunRandomGame(List<Game> gameList)
        {
            // get random game
            var r = new Random();
            var randomIndex = r.Next(0, gameList.Count - 1);
            var randomGame = gameList[randomIndex];
            return RunGame(randomGame);
        }

        /// <summary>
        /// Runs the process
        /// </summary>
        /// <param name="game"></param>
        /// <returns>The <see cref="Process"/> running the game</returns>
        private Process RunGame(Game game)
        {
            // Set the game name and details on the background form
            FrmBackground.SetGameText(game.Description, $@"{game.Year} {game.Manufacturer}");

            Log.Information("Running game {description} {year} {manufacturer} on display {display}", game.Description, game.Year, game.Manufacturer, Screen.DeviceName);

            Application.DoEvents();

            if (!_runGame) return null;

            // Start the timer and the process
            _timer.Start();

            // Create layout and run game
            var artPath = _layoutBuilder.EnsureLayout(game, Screen.Bounds.Width, Screen.Bounds.Height);
            return MameInvoker.Run(game.Name, Settings.CommandLineOptions, "-artpath", artPath, $"-screen \"{Screen.DeviceName}\"");
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _layoutBuilder?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
