using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Mamesaver
{
    public class MameScreen : BlankScreen
    {
        private readonly List<Game> _gameList;
        private readonly bool _runGame;

        Timer _timer;
        private Process _gameProcess;
        private readonly object _syncLock = new object();

        public MameScreen(Screen screen, List<Game> gameList, Action<BlankScreen> onClosed, bool runGame) : base(screen, onClosed)
        {
            _gameList = gameList;
            _runGame = runGame;
        }

        public override void Initialise()
        {
            base.Initialise();

            // Set up the timer
            var minutes = Settings.Minutes;
            _timer = new Timer{ Interval = minutes * 60000 };
            _timer.Tick += timer_Tick;

            FrmBackground.Load += OnFormBackground_Load;
        }

        void OnFormBackground_Load(object sender, EventArgs e)
        {
            // Start the first game
            _gameProcess = RunRandomGame(_gameList);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            // End the currently playing game
            if (_gameProcess != null && !_gameProcess.HasExited) _gameProcess.CloseMainWindow();

            // Start new game
            _gameProcess = RunRandomGame(_gameList);
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

                    if (_gameProcess != null && !_gameProcess.HasExited) _gameProcess.CloseMainWindow();
                }
                catch (Exception)
                {
                    // do nothing
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
            FrmBackground.lblData1.Text = game.Description;
            FrmBackground.lblData2.Text = $@"{game.Year} {game.Manufacturer}";
            
#if DEBUG
            //FrmBackground.lblData2.Text += $@" {_screen.DeviceName}";
            Program.Log("Running game " + game.Description + " " + game.Year + " " + game.Manufacturer);
#endif
            Application.DoEvents();

            if (!_runGame) return null; // mock

            // Start the timer and the process
            _timer.Start();
            return MameInvoker.Run(game.Name, Settings.CommandLineOptions, $"-screen \"{Screen.DeviceName}\"");
        }
    }
}
