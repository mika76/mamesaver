using System.Windows.Forms;
using Mamesaver.Windows;

namespace Mamesaver.Test.Integration
{
    /// <summary>
    ///     Activity hook to simulate key presses and mouse movement
    /// </summary>
    public class TestActivityHook : IActivityHook
    {
        public event MouseEventHandler OnMouseActivity;
        public event KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event KeyEventHandler KeyUp;

        public void SendKeyPress(char key) => KeyPress?.Invoke(this, new KeyPressEventArgs(key));
        public void SendKeyUp(Keys key) => KeyUp?.Invoke(this, new KeyEventArgs(key));
        public void SendKey(Keys key) => KeyDown?.Invoke(this, new KeyEventArgs(key));
        public void MoveMouse() => OnMouseActivity?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
    }
}