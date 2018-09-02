using System;
using System.Management;
using Serilog;

namespace Mamesaver.Power
{
    /// <summary>
    ///     Listens for power source change events, firing a <see cref="PowerStateChanged"/> event
    ///     if received.
    /// </summary>
    public class PowerEventWatcher
    {
        /// <summary>
        ///     Value of event corresponding to a power state change.
        /// </summary>
        private const ushort PowerStateChange = 10;

        private const string ManagementPath = "root\\CIMV2";
        private const string EventClass = "Win32_PowerManagementEvent";

        private ManagementEventWatcher _managementEventWatcher;
        public event PowerEventHandler PowerStateChanged;

        public delegate void PowerEventHandler(object sender, EventArgs args);

        /// <summary>
        ///     Initialises the power event watcher to fire <see cref="PowerStateChanged"/>
        ///     events on power source change.
        /// </summary>
        public void Initialise()
        {
            Log.Debug("Initialising power event watcher");

            var query = new WqlEventQuery { EventClassName = EventClass };
            var scope = new ManagementScope(ManagementPath);

            _managementEventWatcher = new ManagementEventWatcher(scope, query);
            _managementEventWatcher.EventArrived += PowerEventArrived;
            _managementEventWatcher.Start();
        }

        /// <summary>
        ///     Invoked when a power event has been received.
        /// </summary>
        private void PowerEventArrived(object sender, EventArrivedEventArgs args)
        {
            foreach (var propertyData in args.NewEvent.Properties)
            {
                var id = propertyData?.Value as ushort?;
                if (id == PowerStateChange) PowerStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            _managementEventWatcher?.Stop();
            _managementEventWatcher?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PowerEventWatcher() => Dispose(false);
    }
}