/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Serilog;

namespace Mamesaver
{
    internal class Mamesaver : IDisposable
    {
        private readonly List<BlankScreen> _mameScreens = new List<BlankScreen>();

        private readonly ScreenCloner _screenCloner;
        private readonly Settings _settings;
        private readonly GameList _gameList;
        private readonly BlankScreenFactory _screenFactory;
        private readonly MameInvoker _invoker;
        private readonly MameScreen _mameScreen;

        public Mamesaver(
            Settings settings, 
            GameList gameList, 
            ScreenCloner screenCloner,
            BlankScreenFactory screenFactory,
            MameInvoker invoker,
            MameScreen mameScreen)
        {
            _settings = settings;
            _gameList = gameList;
            _screenCloner = screenCloner;
            _screenFactory = screenFactory;
            _invoker = invoker;
            _mameScreen = mameScreen;
        }

        public void Run()
        {
            try
            {
                var gameList = _gameList.SelectedGames;
                Log.Information("{selected} selected games out of {available} games", gameList.Count, _gameList.Games.Count);

                // Exit run method if there were no selected games
                if (!gameList.Any())
                {
                    Log.Information("No selected games available; screensaver exiting");
                    return;
                }

                // Verify that MAME can be run so we can return immediately if there are errors
                _invoker.Run("-showconfig");
                
                // Initialise primary MAME screen
                _mameScreens.Add(_mameScreen);
                _mameScreen.Initialise(Screen.PrimaryScreen, OnScreenClosed);

                // Initialise all other screens
                foreach (var otherScreen in Screen.AllScreens.Where(s => !Equals(s, Screen.PrimaryScreen)))
                {
                    var blankScreen = _screenFactory.Create();
                    _mameScreens.Add(blankScreen);
                    blankScreen.Initialise(otherScreen, OnScreenClosed);
                }

                // Clone mame screens to other screens if required
                if (_settings.CloneScreen) _screenCloner.Clone(_mameScreens.Where(s => s != _mameScreen).ToList());

                // Run the application
                Application.EnableVisualStyles();

                var allForms = _mameScreens.Select(s => s.BackgroundForm).OfType<Form>().ToList();
                Application.Run(new MultiFormApplicationContext(allForms));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to run screensaver");
                throw;
            }
        }

        private void OnScreenClosed()
        {
            try
            {
                _screenCloner.Stop();

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
            Log.Debug("{class} Dispose()", GetType().Name);

            // Explicitly dispose transient screen cloner
            _screenCloner?.Dispose();
        }
    }
}