/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mamesaver.Layout;

namespace Mamesaver
{
    public class Mamesaver
    {
        #region Variables
        GameTimer timer = null;
        BackgroundForm frmBackground = null;
        UserActivityHook actHook = null;
        bool cancelled = false;
        LayoutBuilder layoutBuilder;
        #endregion

        #region DLL Imports
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int X, int Y, int width, int height, uint flags);
        #endregion

        #region Constants
        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private static IntPtr HWND_TOP = IntPtr.Zero;
        private const int SWP_SHOWWINDOW = 64; // 0×0040
        #endregion

        #region Interops
        public static int ScreenX
        {
            get { return GetSystemMetrics(SM_CXSCREEN);}
        }

        public static int ScreenY
        {
            get { return GetSystemMetrics(SM_CYSCREEN);}
        }

        public static void SetWinFullScreen(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
        }
        #endregion


        #region Public Methods
        public void ShowConfig()
        {
            ConfigForm frmConfig = new ConfigForm(this);
            Application.EnableVisualStyles();
            Application.Run(frmConfig);
        }

        public void Run()
        {
            try
            {
                using (layoutBuilder = new LayoutBuilder())
                {
                    // Load list and get only selected games from it
                    List<SelectableGame> gameListFull = Settings.LoadGameList();

                    if (!gameListFull.Any()) return;
                    var gameList = gameListFull.Where(game => game.Selected).Cast<Game>().ToList();

                    // Exit run method if there were no selected games
                    if (gameList.Count == 0) return;

                    // Set up the timer
                    int minutes = Settings.Minutes;
                    timer = new GameTimer(minutes * 60000, gameList);
                    timer.Tick += timer_Tick;

                    // Set up the background form
                    Cursor.Hide();
                    frmBackground = new BackgroundForm();
                    frmBackground.Capture = true;
                    frmBackground.Load += frmBackground_Load;

                    // Set up the global hooks
                    actHook = new UserActivityHook();
                    actHook.OnMouseActivity += actHook_OnMouseActivity;
                    actHook.KeyDown += actHook_KeyDown;

                    // Run the application
                    Application.EnableVisualStyles();
                    Application.Run(frmBackground);
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
       }

        #endregion

        #region Event Hanlders
        void frmBackground_Load(object sender, EventArgs e)
        {
            // Start the first game
            timer.Process = RunRandomGame(timer.GameList);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            // End the currently playing game
            if (timer.Process != null && !timer.Process.HasExited) timer.Process.CloseMainWindow();

            // Start new game
            timer.Process = RunRandomGame(timer.GameList);
        }

        void actHook_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }


        void actHook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            Close();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Stop the timer, set cancelled flag, close any current process and close the background form. 
        /// Once this has all been done, the application should end.
        /// </summary>
        private void Close()
        {
            timer.Stop();
            cancelled = true;
            if (timer.Process != null && !timer.Process.HasExited) timer.Process.CloseMainWindow();
            frmBackground.Close();
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
            Random r = new Random();
            int randomIndex = r.Next(0, gameList.Count - 1);
            Game randomGame = gameList[randomIndex];

            return RunGame(randomGame);
        }

        /// <summary>
        /// Runs the process
        /// </summary>
        /// <param name="game"></param>
        /// <returns>The <see cref="Process"/> running the game</returns>
        private Process RunGame(Game game)
        {
            SetWinFullScreen(frmBackground.Handle);

#if DEBUG
            Program.Log("Running game " + game.Description + " " + game.Year + " " + game.Manufacturer);            
#endif

            // Start the timer and the process
            timer.Start();

          
            // Create layout and run game
            var artPath = layoutBuilder.EnsureLayout(game, ScreenX, ScreenY);
            return MameInvoker.Run(game.Name, Settings.CommandLineOptions, "-artpath", artPath);
        }

        #endregion
    }
}