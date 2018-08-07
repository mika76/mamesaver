/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using gma.System.Windows;

namespace Mamesaver
{
    public class Mamesaver
    {
        #region Variables
        GameTimer timer = null;
        BackgroundForm frmBackground = null;
        UserActivityHook actHook = null;
        bool cancelled = false;
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
                // Load list and get only selected games from it
                List<SelectableGame> gameListFull = Settings.LoadGameList();
                List<Game> gameList = new List<Game>();

                if ( gameListFull.Count == 0 ) return;

                foreach (SelectableGame game in gameListFull)
                    if (game.Selected) gameList.Add(game);

                // Exit run method if there were no selected games
                if ( gameList.Count == 0 ) return;

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
            catch(Exception x)
            {
                MessageBox.Show(x.Message, "Error",  MessageBoxButtons.OK , MessageBoxIcon.Error);
            }
        }



        /// <summary>
        /// Returns a <see cref="List{T}"/> of <see cref="SelectableGame"/>s which are read from
        /// the full list and then merged with the verified rom's list. The games which are returned
        /// all have a "good" status on their drivers. This check also eliminates BIOS ROMS.
        /// </summary>
        /// <returns>Returns a <see cref="List{T}"/> of <see cref="SelectableGame"/>s</returns>
        public List<SelectableGame> GetGameList()
        {
            Hashtable verifiedGames = GetVerifiedSets();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GetFullGameList());
            List<SelectableGame> games = new List<SelectableGame>();

            foreach (string key in verifiedGames.Keys)
            {
                XmlNode xmlGame = doc.SelectSingleNode(string.Format("//machine[@name='{0}']", key));

                if ( xmlGame != null && xmlGame["driver"] != null && xmlGame["driver"].Attributes["status"].Value == "good" )
                    games.Add(new SelectableGame(xmlGame.Attributes["name"].Value, xmlGame["description"].InnerText, xmlGame["year"] != null ? xmlGame["year"].InnerText : "", xmlGame["manufacturer"] != null ? xmlGame["manufacturer"].InnerText : "", false));
            }

            return games;
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
            // Set the game name and details on the background form
            frmBackground.lblData1.Text = game.Description;
            frmBackground.lblData2.Text = game.Year + " " + game.Manufacturer;
            SetWinFullScreen(frmBackground.Handle);

#if DEBUG
            Program.Log("Running game " + game.Description + " " + game.Year + " " + game.Manufacturer);            
#endif

            // Show the form for a couple of seconds
            DateTime end = DateTime.Now.AddSeconds(Settings.BackgroundSeconds);
            while (DateTime.Now < end)
            {
                if (cancelled) return null;
                Application.DoEvents();
            }

            // Set up the process
            string execPath = Settings.ExecutablePath;
            ProcessStartInfo psi = new ProcessStartInfo(execPath);
            psi.Arguments = game.Name + " " + Settings.CommandLineOptions;
            psi.WorkingDirectory = Directory.GetParent(execPath).ToString();
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            // Start the timer and the process
            timer.Start();
            return Process.Start(psi);
        }

        /// <summary>
        /// Gets the full XML game list from <a href="http://www.mame.org/">Mame</a>.
        /// </summary>
        /// <returns><see cref="String"/> holding the Mame XML</returns>
        private string GetFullGameList()
        {
            string execPath = Settings.ExecutablePath;
            ProcessStartInfo psi = new ProcessStartInfo(execPath);
            psi.Arguments = "-listxml";
            psi.WorkingDirectory = Directory.GetParent(execPath).ToString();
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            return output;
        }

        /// <summary>
        /// Returns a <see cref="Hashtable"/> filled with the names of games which are
        /// verified to work. Only the ones marked as good are returned. The clone names
        /// are returned in the value of the hashtable while the name is used as the key.
        /// </summary>
        /// <returns><see cref="Hashtable"/></returns>
        private Hashtable GetVerifiedSets()
        {
            string execPath = Settings.ExecutablePath;
            ProcessStartInfo psi = new ProcessStartInfo(execPath);
            psi.Arguments = "-verifyroms";
            psi.WorkingDirectory = Directory.GetParent(execPath).ToString();
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Process p = Process.Start(psi);
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            Regex r = new Regex(@"romset (\w*)(?:\s\[(\w*)\])? is good"); //only accept the "good" ROMS
            MatchCollection matches = r.Matches(output);
            Hashtable verifiedGames = new Hashtable();

            foreach (Match m in matches)
            {
                verifiedGames.Add(m.Groups[1].Value, m.Groups[2].Value);
            }

            return verifiedGames;
        }
        #endregion
    }
}
