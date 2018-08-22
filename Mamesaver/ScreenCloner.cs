using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Serilog;

namespace Mamesaver
{
    public class ScreenCloner : IDisposable
    {
        private readonly List<BlankScreen> _blankScreens;
        private readonly MameScreen _sourceScreen;
        private readonly Timer _refreshTimer;
        private CaptureScreen _captureScreen;

        public ScreenCloner(MameScreen mameScreen, List<BlankScreen> blankScreens)
        {
            _sourceScreen = mameScreen;
            _blankScreens = blankScreens;

            // is there anything to clone?
            if (_sourceScreen == null || !_blankScreens.Any())
            {
                Log.Information("No source or destinations screens to clone");
                return;
            }

            Log.Information("Source Mame screen found {device} {bounds}", _sourceScreen.Screen.DeviceName, _sourceScreen.Screen.Bounds);
            _blankScreens.ForEach(screen => Log.Information("Destination screen found {device} {bounds}", screen.Screen.DeviceName, screen.Screen.Bounds));

            _refreshTimer = new Timer
            {
                Enabled = true,
                Interval = 1000 / 30 // fps - todo verify cpu usage
            };
            _refreshTimer.Tick += _refreshTimer_Tick;
        }

        public void Dispose()
        {
            _sourceScreen?.Dispose();
            _refreshTimer?.Dispose();
            _captureScreen?.Dispose();
        }

        public void Stop()
        {
            _refreshTimer?.Stop();
            _captureScreen?.Dispose();
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_sourceScreen.GameProcess?.MainWindowHandle == null) return;

            try
            {
                if (_captureScreen == null) _captureScreen = new CaptureScreen();

                _blankScreens.ForEach(screen =>
                {
                    Log.Debug($"Cloning to screen {screen.Screen.DeviceName}");
                    _captureScreen.CloneTo(screen.HandleDeviceContext, screen.Screen.Bounds);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error cloning screens on timer");
            }
        }
    }
}
