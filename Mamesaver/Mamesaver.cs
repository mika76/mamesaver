/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mamesaver
{
    public class Mamesaver
    {
        private readonly List<MameScreen> _mameScreens = new List<MameScreen>();

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
                var gameListFull = Settings.LoadGameList();
                var gameList = new List<Game>();

                if (gameListFull.Count == 0) return;

                foreach (var game in gameListFull)
                {
                    if (game.Selected) gameList.Add(game);
                }

                // Exit run method if there were no selected games
                if (gameList.Count == 0) return;

                foreach (var screen in Screen.AllScreens)
                {
                    var mameScreen = new MameScreen(screen, gameList, OnScreenClosed, true);
                    _mameScreens.Add(mameScreen);
                    mameScreen.Initialise();
                }

                // Run the application
                Application.EnableVisualStyles();
                var allForms = _mameScreens.Select(s => s.FrmBackground).OfType<Form>().ToList();
                Application.Run(new MultiFormApplicationContext(allForms));
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message, @"Error",  MessageBoxButtons.OK , MessageBoxIcon.Error);
            }
        }

        private void OnScreenClosed(MameScreen mameScreen)
        {
            try
            {
                // one screen has closed so close them all
                foreach (var screen in new List<MameScreen>(_mameScreens))
                {
                    _mameScreens.Remove(screen);
                    screen.Close();
                }
            }
            catch (Exception)
            {
                // do nothing as we are closing
            }

            Application.DoEvents();
            Application.Exit();
        }
    }
}
