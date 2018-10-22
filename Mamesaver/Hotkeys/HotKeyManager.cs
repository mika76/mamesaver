using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mamesaver.Models.Configuration;
using Mamesaver.Power;
using Mamesaver.Services.Windows;
using Serilog;

namespace Mamesaver.HotKeys
{
    /// <summary>
    ///     Handles key down events, identifying hot keys and dispatching events for both handled
    ///     and unhandled keys. If hotkeys are disabled, <see cref="UnhandledKeyPressed"/> is published
    ///     for all key presses.
    /// </summary>
    public class HotKeyManager
    {
        /// <summary>
        ///     Hotkey mappings
        /// </summary>
        private readonly IDictionary<Keys, HotKey> _keyMapping = new Dictionary<Keys, HotKey>
        {
            { Keys.Left, HotKey.PreviousGame },
            { Keys.Right, HotKey.NextGame },
            { Keys.Delete, HotKey.DeselectGame },
            { Keys.Enter, HotKey.PlayGame }
        };

        public delegate void HotKeyEventHandler(object sender, HotKeyEventArgs e);

        public event HotKeyEventHandler HotKeyPressed;
        public event EventHandler UnhandledKeyPressed;

        private readonly IActivityHook _activityHook;
        private readonly Settings _settings;
        private readonly PowerManager _powerManager;

        public HotKeyManager(IActivityHook activityHook, Settings settings, PowerManager powerManager)
        {
            _activityHook = activityHook;
            _settings = settings;
            _powerManager = powerManager;
        }

        public void Initialise()
        {
            Log.Debug("Initialising hotkey manager");
            _activityHook.KeyDown += OnKeyDown;
        }

        /// <summary>
        ///     Handles key down events, dispatching events for both hotkeys and unhandled key presses.
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;

            // Reset display sleep timer
            _powerManager.ResetTimer();

            if (_settings.HotKeys && _keyMapping.ContainsKey(key))
            {
                var hotKey = _keyMapping[key];
                Log.Debug("Hotkey {hotkey} pressed", hotKey);

                // Unsubscribe to key events if the user is playing the game as control is shifted to MAME
                if (hotKey == HotKey.PlayGame) _activityHook.KeyDown -= OnKeyDown;

                // Publish hotkey event
                HotKeyPressed?.Invoke(sender, new HotKeyEventArgs(hotKey));
            }
            else
            {
                // Dispatch unhandled keypress event for unmapped keys
                Log.Debug("Unhandled key {key} pressed", key);
                UnhandledKeyPressed?.Invoke(sender, e);
            }
        }
    }
}