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
        private readonly List<BlankScreen> _mameScreens = new List<BlankScreen>();

        public void ShowConfig()
        {
            var frmConfig = new ConfigForm(this);
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

                var showGameOnAllScreens = false; // disabled for now - this only works with opengl

                var mameScreen = new MameScreen(Screen.PrimaryScreen, gameList, OnScreenClosed, true);
                _mameScreens.Add(mameScreen);
                mameScreen.Initialise();

                foreach (var otherScreen in Screen.AllScreens.Where(s => s != Screen.PrimaryScreen))
                {
                    var blankScreen = showGameOnAllScreens?
                        new MameScreen(otherScreen, gameList, OnScreenClosed, true) :
                        new BlankScreen(otherScreen, OnScreenClosed);
                    _mameScreens.Add(blankScreen);
                    blankScreen.Initialise();
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

        private void OnScreenClosed(BlankScreen mameScreen)
        {
            try
            {
                // one screen has closed so close them all
                foreach (var screen in new List<BlankScreen>(_mameScreens))
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
