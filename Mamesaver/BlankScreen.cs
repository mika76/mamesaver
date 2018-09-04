using System;
using System.Windows.Forms;
using Mamesaver.Power;
using Mamesaver.Windows;
using Serilog;
using static Mamesaver.Windows.MonitorInterop;
using static Mamesaver.Windows.PlatformInvokeUser32;
using static Mamesaver.Windows.PlatformInvokeGdi32;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;

namespace Mamesaver
{
    internal class BlankScreen
    {
        private readonly PowerManager _powerManager;
        public BackgroundForm BackgroundForm { get; }

        public Screen Screen { get; private set; }
        public IntPtr HandleDeviceContext { get; private set; } = IntPtr.Zero;

        private int _xDpi, _yDpi;

        public BlankScreen(LayoutSettings layoutSettings, PowerManager powerManager)
        {
            _powerManager = powerManager;
            BackgroundForm = new BackgroundForm(layoutSettings);
        }

        public virtual void Initialise(Screen screen)
        {
            Screen = screen;

            BackgroundForm.Load += BackgroundForm_Load;
            BackgroundForm.primaryLabel.Text = string.Empty;
            BackgroundForm.secondaryLabel.Text = string.Empty;
            BackgroundForm.mameLogo.Visible = false;
            BackgroundForm.FormClosing += (sender, args) => ReleaseDeviceContext();

            _powerManager.SleepTriggered += OnSleep;

            Cursor.Hide();
        }

        /// <summary>
        ///     Hide the background form elements to provide seamless transition between games or after MAME termination.
        /// </summary>
        protected void HideBackgroundForm()
        {
            BackgroundForm.HideAll();

            // Force an immediate refresh to avoid flicker of the MAME logo
            BackgroundForm.Refresh();
        }

        private void OnSleep(object sender, EventArgs e)
        {
            Log.Information("Sleeping screen {screen}", Screen.DeviceName);
            SetMonitorState(BackgroundForm.Handle, MonitorState.MonitorStateOff);
        }

        private void BackgroundForm_Load(object sender, EventArgs e)
        {
            HandleDeviceContext = GetDC(BackgroundForm.Handle);

            _xDpi = GetDeviceCaps(HandleDeviceContext, (int)DeviceCap.LOGPIXELSX);
            _yDpi = GetDeviceCaps(HandleDeviceContext, (int)DeviceCap.LOGPIXELSY);

            // 96 is the default dpi for windows 
            // https://docs.microsoft.com/en-us/windows/desktop/directwrite/how-to-ensure-that-your-application-displays-properly-on-high-dpi-displays
            var width = _xDpi * Screen.Bounds.Width / 96f;
            var height = _yDpi * Screen.Bounds.Height / 96f;
            WindowsInterop.SetWinFullScreen(BackgroundForm.Handle, Screen.Bounds.Left, Screen.Bounds.Top, (int) width, (int)height);

            Log.Information("Blank screen resized {device} {bounds} xDpi {xDpi} yDpi {yDpi}", Screen.DeviceName, Screen.Bounds, _xDpi, _yDpi);
        }

        private void ReleaseDeviceContext()
        {
            HideBackgroundForm();

            try
            {
                if (HandleDeviceContext == IntPtr.Zero)
                {
                    Log.Debug("No device context to release for {screen}", Screen.DeviceName);
                    return;
                }

                Log.Debug("Releasing device context for {screen}", Screen.DeviceName);

                ReleaseDC(BackgroundForm.Handle, HandleDeviceContext);
                HandleDeviceContext = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error releasing device context for {screen}", Screen.DeviceName);
            }
        }
    }
}