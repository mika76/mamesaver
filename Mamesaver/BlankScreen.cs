using System;
using System.Windows.Forms;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver
{
    internal class BlankScreen : IDisposable
    {
        public BackgroundForm BackgroundForm { get; }

        private bool _disposed;
        private Action _onClosed;
        private UserActivityHook _actHook;
        private bool _cancelled;
        private readonly object _syncLock = new object();
        public Screen Screen { get; private set; }
        public IntPtr HandleDeviceContext { get; private set; } = IntPtr.Zero;

        public BlankScreen(BackgroundForm backgroundForm) => BackgroundForm = backgroundForm;

        public virtual void Initialise(Screen screen, Action onClosed)
        {
            Screen = screen;
            _onClosed = onClosed;

            BackgroundForm.Load += BackgroundForm_Load;
            BackgroundForm.primaryLabel.Text = string.Empty;
            BackgroundForm.secondaryLabel.Text = string.Empty;
            BackgroundForm.mameLogo.Visible = false;

            Cursor.Hide();

            // Set up the global hooks
            _actHook = new UserActivityHook();
            _actHook.OnMouseActivity += actHook_OnMouseActivity;
            _actHook.KeyDown += actHook_KeyDown;
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

        void actHook_KeyDown(object sender, KeyEventArgs e) => Close();
        void actHook_OnMouseActivity(object sender, MouseEventArgs e) => Close();

        public virtual void Close()
        {

            lock (_syncLock)
            {
                try
                {
                    if (_cancelled) return;
                    Log.Information("Closing screen {screen}", Screen.DeviceName);

                    _actHook?.Stop();
                    Cursor.Show();

                    ReleaseUnmanagedResources();
                    BackgroundForm?.Close();

                    _cancelled = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error closing");
                }
            }

            _onClosed();
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            ReleaseUnmanagedResources();

            _disposed = true;
        }

        public void Dispose()
        {
            Log.Debug("{class} Dispose()", GetType().Name);

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BlankScreen()
        {
            Dispose(false);
        }

        private void ReleaseUnmanagedResources()
        {
            try
            {
                if (HandleDeviceContext != IntPtr.Zero)
                {
                    PlatformInvokeUser32.ReleaseDC(BackgroundForm.Handle, HandleDeviceContext);
                    HandleDeviceContext = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error releasing unmanaged resources");
            }
        }
    }
}