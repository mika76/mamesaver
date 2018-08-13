using System;
using System.Windows.Forms;
using Mamesaver.Windows;

namespace Mamesaver
{
    public class BlankScreen
    {
        private readonly Action<BlankScreen> _onClosed;
        private UserActivityHook _actHook;
        private bool _cancelled;
        private readonly object _syncLock = new object();
        private readonly bool _debugging = false;

        public BlankScreen(Screen screen, Action<BlankScreen> onClosed)
        {
            Screen = screen;
            _onClosed = onClosed;
        }

        public Screen Screen { get; }

        public BackgroundForm FrmBackground { get; private set; }

        public virtual void Initialise()
        {
            FrmBackground = new BackgroundForm { Capture = !_debugging };
            FrmBackground.Load += FrmBackground_Load;
            FrmBackground.lblData1.Text = string.Empty;
            FrmBackground.lblData2.Text = string.Empty;

            // Set up the global hooks
            _actHook = new UserActivityHook();
            if (!_debugging)
            {
                Cursor.Hide();

                _actHook.OnMouseActivity += actHook_OnMouseActivity;
                _actHook.KeyDown += actHook_KeyDown;
            }
        }

        private void FrmBackground_Load(object sender, EventArgs e)
        {
            if (!_debugging)
                WindowsInterop.SetWinFullScreen(FrmBackground.Handle, Screen.Bounds.Left, Screen.Bounds.Top, Screen.Bounds.Width, Screen.Bounds.Height);
        }

        void actHook_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }


        void actHook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            Close();
        }

        public virtual void Close()
        {
            lock (_syncLock)
            {
                try
                {
                    if (_cancelled) return;

                    _cancelled = true;
                    Cursor.Show();
                    FrmBackground?.Close();
                    FrmBackground = null;
                }
                catch (Exception)
                {
                    // do nothing
                }
            }

            _onClosed(this);
        }
    }
}
