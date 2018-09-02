using System;
using System.Windows.Forms;
using Mamesaver.Power;
using Mamesaver.Windows;
using Serilog;
using static Mamesaver.Windows.MonitorInterop;
using static Mamesaver.Windows.PlatformInvokeUser32;
using LayoutSettings = Mamesaver.Configuration.Models.LayoutSettings;

namespace Mamesaver
{
    internal class BlankScreen
    {
        private readonly PowerManager _powerManager;
        public BackgroundForm BackgroundForm { get; }

        public Screen Screen { get; private set; }
        public IntPtr HandleDeviceContext { get; private set; } = IntPtr.Zero;

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
            BackgroundForm.Disposed += (sender, args) => ReleaseDeviceContext();

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
            WindowsInterop.SetWinFullScreen(BackgroundForm.Handle, Screen.Bounds.Left, Screen.Bounds.Top, Screen.Bounds.Width, Screen.Bounds.Height);
            HandleDeviceContext = GetDC(BackgroundForm.Handle);
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