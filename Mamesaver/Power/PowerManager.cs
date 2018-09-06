using System;
using System.Linq;
using System.Timers;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver.Power
{
    /// <summary>
    ///     Identifies screen and PC power management configuration, firing a single <see cref="SleepTriggered"/> event 
    ///     when the either require sleeping. This explicit power management is required because MAME prevents both 
    ///     the display and the PC from going to sleep.
    /// </summary>
    /// <remarks>
    ///     Currently, the <see cref="SleepTriggered"/> event doesn't differentiate between screen and PC power 
    ///     saving, nor does it contain any information about the requested action for PC power management. It is
    ///     expected that either will only be used to turn the screen off and terminate MAME.
    /// </remarks>
    public class PowerManager : IDisposable
    {
        private readonly PowerEventWatcher _eventWatcher;
        private Timer _sleepTimer;

        public delegate void SleepTriggerManager(object sender, EventArgs args);

        /// <summary>
        ///     Fired when the screensaver should turn the display to sleep.
        /// </summary>
        public event SleepTriggerManager SleepTriggered;

        /// <summary>
        ///     Time before a <see cref="SleepTriggered"/> event is published, based on the user's power management 
        ///     configuration.
        /// </summary>
        private TimeSpan _sleepTimeout;

        /// <summary>
        ///     Current power source type.
        /// </summary>
        private PowerType _currentPowerType;

        public PowerManager(PowerEventWatcher eventWatcher) => _eventWatcher = eventWatcher;

        /// <summary>
        ///     Resets the sleep timer.
        /// </summary>
        public void ResetTimer()
        {
            if (_sleepTimer == null) return;

            Log.Debug("Resetting sleep timer");
            _sleepTimer.Stop();
            _sleepTimer.Start();
        }

        /// <summary>
        ///     Initialises the power manager to publish <see cref="PowerStateChanged"/> events when power state
        ///     has changed from AC to DC or vice versa.
        /// </summary>
        public void Initialise()
        {
            Log.Debug("Initialising power manager");

            // Identify whether we are on AC or DC power
            if (!GetPowerType(out var powerType) || powerType == null)
            {
                Log.Warning("Unable to determine power source; unable to perform power management");
                return;
            }

            _currentPowerType = powerType.Value;

            // Identify policy for selected power type
            _sleepTimeout = GetSleepTimeout(powerType.Value);
            Log.Information("Connected to {powerType} power; sleeping after {min} minutes", 
                powerType.Value.ToString(), _sleepTimeout.TotalMinutes);

            // Create a timer which fires once when the display or computer should go to sleep
            _sleepTimer = new Timer { Interval = Math.Min(_sleepTimeout.TotalMilliseconds, int.MaxValue), AutoReset = false };

            _sleepTimer.Elapsed += SleepTimerTick;
            _sleepTimer.Start();

            // Receive notifications for power state changes so the timer can be updated
            _eventWatcher.Initialise();
            _eventWatcher.PowerStateChanged += PowerStateChanged;
        }

        /// <summary>
        ///     Returns the lowest of screen and PC sleep settings for a given power type.
        /// </summary>
        private static TimeSpan GetSleepTimeout(PowerType powerType)
        {
            var powerPolicy = PowerInterop.GetPowerPolicy(powerType);
            return new[] { powerPolicy.VideoTimeout, powerPolicy.IdleTimeout }.Min();
        }

        /// <summary>
        ///     Retrieves the current power source.
        /// </summary>
        private static bool GetPowerType(out PowerType? powerType)
        {
            powerType = PowerInterop.GetPowerType();
            return powerType != null;
       }

        /// <summary>
        ///     Invoked when the power state has changed from AC to DC or vice versa.
        /// </summary>
        private void PowerStateChanged(object sender, EventArgs args)
        {
            if (!GetPowerType(out var powerType) || powerType == null)
            {
                Log.Warning("Unable to determine power source; not reacting to power state change");
                return;
            }

            // Multiple identical state change events can be triggered; return if there's nothing to do
            if (_currentPowerType == powerType) return;
            _currentPowerType = powerType.Value;

            // Identify policy for selected power type
            _sleepTimeout = GetSleepTimeout(powerType.Value);
            Log.Information("Power changed to {powerType} power; sleeping after {min} minutes",
                powerType.Value.ToString(), _sleepTimeout.TotalMinutes);

            // Update sleep timer on power state change due to different sleep configurations
            _sleepTimer.Interval = Math.Min(_sleepTimeout.TotalMilliseconds, int.MaxValue);

            _sleepTimer.Stop();
            _sleepTimer.Start();
        }

        /// <summary>
        ///     Publishes a <see cref="SleepTriggered"/> event when either the display or PC require sleeping.
        /// </summary>
        private void SleepTimerTick(object sender, EventArgs e) => SleepTriggered?.Invoke(this, new EventArgs());

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _sleepTimer?.Dispose();
        }

        ~PowerManager() => Dispose(false);
    }
}