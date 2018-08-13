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
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            if (_sourceScreen.GameProcess?.MainWindowHandle == null) return;

            try
            {
                // todo test different size monitors
                //PlatformInvokeUser32.RECT rect;
                //PlatformInvokeUser32.GetWindowRect(_sourceScreen.GameProcess.MainWindowHandle, out rect);
                //var bitMap = CaptureScreen.Capture(_sourceScreen.GameProcess.MainWindowHandle, rect.Right - rect.Left, rect.Top - rect.Bottom);
                var bitMap = CaptureScreen.GetDesktopImage();

                _blankScreens.ForEach(screen => screen.FrmBackground.BackgroundImage = bitMap);
            }
            catch (Exception)
            {
                // bugger
            }
        }
    }
}
