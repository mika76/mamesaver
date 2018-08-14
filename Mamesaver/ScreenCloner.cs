using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mamesaver
{
    public class ScreenCloner
    {
        private readonly List<BlankScreen> _blankScreens;
        private readonly MameScreen _sourceScreen;
        private readonly Timer _refreshTimer;
        private CaptureScreen _captureScreen;

        public ScreenCloner(List<BlankScreen> allScreens)
        {
            _sourceScreen = allScreens.OfType<MameScreen>().First();
            _blankScreens = allScreens.Where(s => s != _sourceScreen).ToList();

            // is there anything to clone?
            if (_sourceScreen != null && !_blankScreens.Any()) return;

            _refreshTimer = new Timer
            {
                Enabled = true,
                Interval = 1000 / 30 // fps - todo verify cpu usage
            };
            _refreshTimer.Tick += _refreshTimer_Tick;
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

                // todo test different size monitors
                var bitMap = _captureScreen.Capture();

                _blankScreens.ForEach(screen =>
                {
                    screen.FrmBackground.BackgroundImage?.Dispose();
                    screen.FrmBackground.BackgroundImage = bitMap;
                });
            }
            catch (Exception)
            {
                // bugger
            }
        }
    }
}
