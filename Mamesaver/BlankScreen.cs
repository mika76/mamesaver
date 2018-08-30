using System;
using System.Windows.Forms;
using Mamesaver.Configuration.Models;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver
{
    internal class BlankScreen : IDisposable
    {
        private readonly Settings _settings;
        public BackgroundForm BackgroundForm { get; }

        private bool _disposed;
        private Action _onClosed;
        protected UserActivityHook ActivityHook { get; private set; }
        private bool _cancelled;
        private readonly object _syncLock = new object();
        public Screen Screen { get; private set; }
        public IntPtr HandleDeviceContext { get; private set; } = IntPtr.Zero;

        public BlankScreen(BackgroundForm backgroundForm, Settings settings)
        {
            _settings = settings;
            BackgroundForm = backgroundForm;
        }

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
            ActivityHook = new UserActivityHook();
            BindActivityHooks();
        }

        protected void BindActivityHooks()
        {
            ActivityHook.OnMouseActivity += actHook_OnMouseActivity;
            ActivityHook.KeyDown += actHook_KeyDown;
        }

        protected void UnbindActivityHooks()
        {
            ActivityHook.OnMouseActivity -= actHook_OnMouseActivity;
            ActivityHook.KeyDown -= actHook_KeyDown;
        }

        private void BackgroundForm_Load(object sender, EventArgs e)
        {
            WindowsInterop.SetWinFullScreen(BackgroundForm.Handle, Screen.Bounds.Left, Screen.Bounds.Top, Screen.Bounds.Width, Screen.Bounds.Height);
            HandleDeviceContext = PlatformInvokeUser32.GetDC(BackgroundForm.Handle);
        }

        /// <summary>
        ///     Close window on key press if hotkeys aren't enabled.
        /// </summary>
        public virtual void actHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_settings.HotKeys) Close();
        }

        void actHook_OnMouseActivity(object sender, MouseEventArgs e) => Close();

        public virtual void Close()
        {

            lock (_syncLock)
            {
                try
                {
                    if (_cancelled) return;
                    Log.Information("Closing screen {screen}", Screen.DeviceName);

                    ActivityHook?.Stop();
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