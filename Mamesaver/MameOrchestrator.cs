/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Mamesaver.Hotkeys;
using Mamesaver.Power;
using Serilog;

namespace Mamesaver
{
    internal class MameOrchestrator  : IDisposable
    {
        private readonly ScreenCloner _screenCloner;
        private readonly ScreenManager _screenManager;
        private readonly HotKeyManager _hotKeyManager;
        private readonly PowerManager _powerManager;
        private readonly Settings _settings;
        private readonly GameList _gameList;
        private readonly BlankScreenFactory _screenFactory;
        private readonly MameInvoker _invoker;
        private readonly GamePlayManager _gamePlayManager;
        private readonly MameScreen _mameScreen;
        private CancellationTokenSource _cancellationTokenSource;

        public MameOrchestrator(
            Settings settings,
            GameList gameList,
            ScreenCloner screenCloner,
            ScreenManager screenManager,
            HotKeyManager hotKeyManager,
            PowerManager powerManager,
            BlankScreenFactory screenFactory,
            MameInvoker invoker,
            GamePlayManager gamePlayManager,
            MameScreen mameScreen)
        {
            _settings = settings;
            _gameList = gameList;
            _screenCloner = screenCloner;
            _screenManager = screenManager;
            _hotKeyManager = hotKeyManager;
            _powerManager = powerManager;
            _screenFactory = screenFactory;
            _invoker = invoker;
            _gamePlayManager = gamePlayManager;
            _mameScreen = mameScreen;
        }

        public void Run()
        {
            try
            {
                _cancellationTokenSource = new CancellationTokenSource();
                 _cancellationTokenSource.Token.Register(Stop);

                var gameList = _gameList.SelectedGames;
                Log.Information("{selected} selected games out of {available} games", gameList.Count, _gameList.Games.Count);

                // Exit run method if there were no selected games
                if (!gameList.Any())
                {
                    Log.Information("No selected games available; screensaver exiting");
                    return;
                }

                // Start listening for user input events
                _screenManager.Initialise(_cancellationTokenSource);
                _hotKeyManager.Initialise();            

                // Start power management timer
                _powerManager.Initialise();

                // Verify that MAME can be run so we can return immediately if there are errors
                _invoker.Run("-showconfig");

                // Find the best primary screen for MAME. As games are largely vertical and screens are wide, select the one with the greatest Y axis
                var bestPrimaryScreen = Screen.AllScreens.OrderByDescending(screen => screen.Bounds.Height).First();

                // Initialise primary MAME screen
                _gamePlayManager.Initialise(bestPrimaryScreen, _cancellationTokenSource);
                _mameScreen.Initialise(bestPrimaryScreen);

                // Initialise all other screens
                var clonedScreens = new List<BlankScreen>();
                foreach (var otherScreen in Screen.AllScreens.Where(screen => !Equals(screen, bestPrimaryScreen)))
                {
                    var blankScreen = _screenFactory.Create();
                    _screenManager.RegisterScreen(blankScreen);

                    blankScreen.Initialise(otherScreen);
                    clonedScreens.Add(blankScreen);
                }

                // Clone mame screens to other screens if required
                if (_settings.CloneScreen) _screenCloner.StartCloning(clonedScreens);

                // Run the application
                Application.EnableVisualStyles();

                var allForms = clonedScreens
                    .Concat(new List<BlankScreen> { _mameScreen })
                    .Select(s => s.BackgroundForm)
                    .OfType<Form>()
                    .ToList();

                var context = new MultiFormApplicationContext(allForms);

                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Application.Run(context);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to run screensaver");
                throw;
            }
        }

        /// <summary>
        ///     Stops the screensaver
        /// </summary>
        private void Stop()
        {
            try
            {
                Log.Information("Stopping screensaver");
                Application.Exit();
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to gracefully exit");
                Environment.Exit(-1);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _cancellationTokenSource?.Dispose();
        }

        ~MameOrchestrator() => Dispose(false);
    }
}