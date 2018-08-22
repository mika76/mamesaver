/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Serilog;

namespace Mamesaver
{
    public class Mamesaver : IDisposable
    {
        private readonly List<BlankScreen> _mameScreens = new List<BlankScreen>();
        private ScreenCloner _screenCloner;

        public void Run()
        {
            try
            {
                // Load list and get only selected games from it
                var gameListFull = Settings.LoadGameList();

                if (!gameListFull.Any()) return;
                var gameList = gameListFull.Where(game => game.Selected).Cast<Game>().ToList();

                // Exit run method if there were no selected games
                if (gameList.Count == 0) return;

                var mameScreen = new MameScreen(Screen.PrimaryScreen, gameList, OnScreenClosed, true);
                _mameScreens.Add(mameScreen);
                mameScreen.Initialise();

                foreach (var otherScreen in Screen.AllScreens.Where(s => !Equals(s, Screen.PrimaryScreen)))
                {
                    var blankScreen = new BlankScreen(otherScreen, OnScreenClosed);
                    _mameScreens.Add(blankScreen);
                    blankScreen.Initialise();
                }

                if (Settings.CloneScreen) _screenCloner = new ScreenCloner(mameScreen, _mameScreens.Where(s => s != mameScreen).ToList());

                // Run the application
                Application.EnableVisualStyles();
                var allForms = _mameScreens.Select(s => s.FrmBackground).OfType<Form>().ToList();
                Application.Run(new MultiFormApplicationContext(allForms));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnScreenClosed(BlankScreen mameScreen)
        {
            try
            {
                _screenCloner?.Stop();

                // one screen has closed so close them all
                foreach (var screen in new List<BlankScreen>(_mameScreens))
                {
                    _mameScreens.Remove(screen);
                    screen.Close();
                    screen.Dispose();
                }
            }
            catch (Exception ex)
            {
                // do nothing as we are closing
                Log.Error(ex, "Error closing screens");
            }

            Application.DoEvents();
            Application.Exit();
        }

        public void Dispose()
        {
            _screenCloner?.Dispose();
        }
    }
}