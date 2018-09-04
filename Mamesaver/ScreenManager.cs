using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Mamesaver.Hotkeys;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver
{
    /// <summary>
    ///     Handles mouse and keyboard activity which affects screens.
    /// </summary>
    internal class ScreenManager
    {
        private readonly ScreenCloner _screenCloner;
        private readonly HotKeyManager _hotKeyManager;
        private readonly IActivityHook _activityHook;

        /// <summary>
        ///     List populated with screens cloned from <see cref="MameScreen"/>
        /// </summary>
        private readonly List<BlankScreen> _screens = new List<BlankScreen>();

        private Action _onClose;
        private CancellationToken _cancellationToken;

        public ScreenManager(ScreenCloner screenCloner, HotKeyManager hotKeyManager, IActivityHook activityHook)
        {
            _screenCloner = screenCloner;
            _hotKeyManager = hotKeyManager;
            _activityHook = activityHook;
        }

        public void Initialise(CancellationToken cancellationToken, Action onClose)
        {
            Log.Debug("Initialising screen manager");

            _cancellationToken = cancellationToken;
            _onClose = onClose;

            _hotKeyManager.HotKeyPressed += HotKeyHandler;
            _hotKeyManager.UnhandledKeyPressed += UnhandledKeyPressed;
            _activityHook.OnMouseActivity += OnMouseActivity;
        }

        /// <summary>
        ///     Registers a screen for lifecycle management.
        /// </summary>
        /// <param name="screen"></param>
        public void RegisterScreen(BlankScreen screen)
        {
            if (screen == null) throw new ArgumentNullException(nameof(screen));
            if (screen is MameScreen) throw new ArgumentException("Only unmanaged cloned screens are supported");

            _screens.Add(screen);
        }

        /// <summary>
        ///     Exits screensaver on mouse activity.
        /// </summary>
        private void OnMouseActivity(object sender, MouseEventArgs e)
        {
            // Log for first mouse activity
            if (!_cancellationToken.IsCancellationRequested) Log.Debug("Exiting due to mouse activity");
            _onClose();
        }

        /// <summary>
        ///    Exits screensaver on keypress which isn't handled by hotkeys.  
        /// </summary>
        private void UnhandledKeyPressed(object sender, EventArgs e)
        {
            // Log for first keypress
            if (!_cancellationToken.IsCancellationRequested) Log.Debug("Exiting due to unhandled keypress");
            _onClose();
        }

        /// <summary>
        ///     Handles hotkey events which affect screen lifecycle.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HotKeyHandler(object sender, HotKeyEventArgs args)
        {
            // Unsubscribe to all mouse and keyboard events and close all clone screens  when the user 
            // is playing a game.
            if (args.HotKey == HotKey.PlayGame)
            {
                _hotKeyManager.HotKeyPressed -= HotKeyHandler;
                _hotKeyManager.UnhandledKeyPressed -= UnhandledKeyPressed;
                _activityHook.OnMouseActivity -= OnMouseActivity;

                // Stop cloning to screens and refresh form to repaint background
                _screenCloner.Stop();
                _screens.ForEach(screen => screen.BackgroundForm.Refresh());
            }
        }
    }
}