using System;
using System.Windows.Forms;

namespace Mamesaver
{
    public class CloneScreen : BlankScreen
    {
        private readonly MameScreen _sourceScreen;
        private Timer _refreshTimer;

        public CloneScreen(Screen screen, MameScreen sourceScreen, Action<BlankScreen> onCloseAction) : base(screen,
            onCloseAction)
        {
            _sourceScreen = sourceScreen;
        }

        public override void Initialise()
        {
            base.Initialise();

            _refreshTimer = new Timer
            {
                Enabled = true,
                Interval = 1000 / 30 // fps
            };
            _refreshTimer.Tick += _refreshTimer_Tick;
        }

        private void _refreshTimer_Tick(object sender, EventArgs e)
        {
            if (FrmBackground == null || _sourceScreen.GameProcess?.MainWindowHandle == null) return;

            try
            {
                //PlatformInvokeUser32.RECT rect;
                //PlatformInvokeUser32.GetWindowRect(_sourceScreen.GameProcess.MainWindowHandle, out rect);
                //var bitMap = CaptureScreen.Capture(_sourceScreen.GameProcess.MainWindowHandle, rect.Right - rect.Left, rect.Top - rect.Bottom);
                var bitMap = CaptureScreen.GetDesktopImage();
                FrmBackground.BackgroundImage = bitMap;
            }
            catch (Exception)
            {
                // how rude
            }
        }
    }
}
