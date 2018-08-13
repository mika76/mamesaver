using System;
using System.Windows.Forms;
using gma.System.Windows;

namespace Mamesaver
{
    public class BlankScreen
    {
        private readonly Action<BlankScreen> _onClosed;
        private UserActivityHook _actHook;
        private bool _cancelled;
        private readonly object _syncLock = new object();

        public BlankScreen(Screen screen, Action<BlankScreen> onClosed)
        {
            Screen = screen;
            _onClosed = onClosed;
        }

        public Screen Screen { get; }

        public BackgroundForm FrmBackground { get; private set; }

        public virtual void Initialise()
        {
            Cursor.Hide();
            FrmBackground = new BackgroundForm { Capture = true };
            FrmBackground.Load += FrmBackground_Load;
            FrmBackground.lblData1.Text = string.Empty;
            FrmBackground.lblData2.Text = string.Empty;

            // Set up the global hooks
            _actHook = new UserActivityHook();
            _actHook.OnMouseActivity += actHook_OnMouseActivity;
            _actHook.KeyDown += actHook_KeyDown;
        }

        private void FrmBackground_Load(object sender, EventArgs e)
        {
            WindowsInterop.SetWinFullScreen(FrmBackground.Handle, Screen.WorkingArea.Left, Screen.WorkingArea.Top, Screen.WorkingArea.Width, Screen.WorkingArea.Height);
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
