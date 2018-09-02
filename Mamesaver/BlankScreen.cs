using System;
using System.Windows.Forms;
using Mamesaver.Windows;
using Serilog;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;

namespace Mamesaver
{
    internal class BlankScreen
    {
        public BackgroundForm BackgroundForm { get; }

        public Screen Screen { get; private set; }
        public IntPtr HandleDeviceContext { get; private set; } = IntPtr.Zero;

        public BlankScreen(LayoutSettings layoutSettings) => BackgroundForm = new BackgroundForm(layoutSettings);

        public virtual void Initialise(Screen screen)
        {
            Screen = screen;

            BackgroundForm.Load += BackgroundForm_Load;
            BackgroundForm.primaryLabel.Text = string.Empty;
            BackgroundForm.secondaryLabel.Text = string.Empty;
            BackgroundForm.mameLogo.Visible = false;
            BackgroundForm.Disposed += (sender, args) => ReleaseDeviceContext();

            Cursor.Hide();
        }

        public int XDpi { get; private set; }
        public int YDpi { get; private set; }

        private void BackgroundForm_Load(object sender, EventArgs e)
        {
            HandleDeviceContext = PlatformInvokeUser32.GetDC(BackgroundForm.Handle);

            XDpi = PlatformInvokeGdi32.GetDeviceCaps(HandleDeviceContext, (int)PlatformInvokeGdi32.DeviceCap.LOGPIXELSX);
            YDpi = PlatformInvokeGdi32.GetDeviceCaps(HandleDeviceContext, (int)PlatformInvokeGdi32.DeviceCap.LOGPIXELSY);

            // 96 is the default dpi for windows 
            // https://docs.microsoft.com/en-us/windows/desktop/directwrite/how-to-ensure-that-your-application-displays-properly-on-high-dpi-displays
            var width = XDpi * Screen.Bounds.Width / 96f;
            var height = YDpi * Screen.Bounds.Height / 96f;
            WindowsInterop.SetWinFullScreen(BackgroundForm.Handle, Screen.Bounds.Left, Screen.Bounds.Top, (int) width, (int)height);
        }

        private void ReleaseDeviceContext()
        {
            try
            {
                if (HandleDeviceContext == IntPtr.Zero)
                {
                    Log.Debug("No device context to release for {screen}", Screen.DeviceName);
                    return;
                }

                Log.Debug("Releasing device context for {screen}", Screen.DeviceName);

                PlatformInvokeUser32.ReleaseDC(BackgroundForm.Handle, HandleDeviceContext);
                HandleDeviceContext = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error releasing device context for {screen}", Screen.DeviceName);
            }
        }
    }
}