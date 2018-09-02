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

        /// <summary>
        ///     Hide the background form elements to provide seamless transition between games or after MAME termination.
        /// </summary>
        protected void HideBackgroundForm()
        {
            BackgroundForm.HideAll();

            // Force an immediate refresh to avoid flicker of the MAME logo
            BackgroundForm.Refresh();
        }
        private void BackgroundForm_Load(object sender, EventArgs e)
        {
            WindowsInterop.SetWinFullScreen(BackgroundForm.Handle, Screen.Bounds.Left, Screen.Bounds.Top, Screen.Bounds.Width, Screen.Bounds.Height);
            HandleDeviceContext = PlatformInvokeUser32.GetDC(BackgroundForm.Handle);
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