using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using gma.System.Windows;

namespace Mamesaver
{
    public class MameScreen
    {
        private readonly Screen _screen;
        private readonly List<Game> _gameList;
        private readonly Action<MameScreen> _onClosed;
        private readonly bool _runGame;

        Timer _timer;
        private Process _gameProcess;
        public BackgroundForm FrmBackground { get; private set; }
        UserActivityHook _actHook;
        bool _cancelled;

        #region DLL Imports
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);
        #endregion

        private static readonly IntPtr HwndTop = IntPtr.Zero;
        private const int SwpShowwindow = 64; // 0×0040

        
        public static void SetWinFullScreen(IntPtr hwnd, int left, int top, int width, int height)
        {
            SetWindowPos(hwnd, HwndTop, left, top, width, height, SwpShowwindow);
        }

        public MameScreen(Screen screen, List<Game> gameList, Action<MameScreen> onClosed, bool runGame)
        {
            _screen = screen;
            _gameList = gameList;
            _onClosed = onClosed;
            _runGame = runGame;
        }

        public void Initialise()
        {
            Cursor.Hide();

            // Set up the timer
            var minutes = Settings.Minutes;
            _timer = new Timer{ Interval = minutes * 60000 };
            _timer.Tick += timer_Tick;

            FrmBackground = new BackgroundForm { Capture = true };
            FrmBackground.Load += frmBackground_Load;

            // Set up the global hooks
            _actHook = new UserActivityHook();
            _actHook.OnMouseActivity += actHook_OnMouseActivity;
            _actHook.KeyDown += actHook_KeyDown;
        }

        void frmBackground_Load(object sender, EventArgs e)
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

        void actHook_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }


        void actHook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Stop the timer, set cancelled flag, close any current process and close the background form. 
        /// Once this has all been done, the application should end.
        /// </summary>
        public void Close()
        {
            if (_cancelled) return;
            try
            {
                Cursor.Show();
                _timer.Stop();
                _cancelled = true;
                if (_gameProcess != null && !_gameProcess.HasExited) _gameProcess.CloseMainWindow();
            }
            catch (Exception)
            {
                // do nothing
            }
            
            FrmBackground.Close();
            FrmBackground = null;
            _onClosed(this);
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

            SetWinFullScreen(FrmBackground.Handle, _screen.WorkingArea.Left, _screen.WorkingArea.Top, _screen.WorkingArea.Width, _screen.WorkingArea.Height);

#if DEBUG
            //FrmBackground.lblData2.Text += $@" {_screen.DeviceName}";
            Program.Log("Running game " + game.Description + " " + game.Year + " " + game.Manufacturer);
#endif
            Application.DoEvents();

            if (!_runGame) return null; // mock

            // Set up the process
            var execPath = Settings.ExecutablePath;
            var psi = new ProcessStartInfo(execPath)
            {
                Arguments = $"{game.Name} {Settings.CommandLineOptions} -screen \"{_screen.DeviceName}\"",
                WorkingDirectory = Directory.GetParent(execPath).ToString(),
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            
            // Start the timer and the process
            _timer.Start();
             return Process.Start(psi);
        }
    }
}
