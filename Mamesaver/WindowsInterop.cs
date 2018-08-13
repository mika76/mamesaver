using System;
using System.Runtime.InteropServices;

namespace Mamesaver
{
    public static class WindowsInterop
    {
        #region DLL Imports
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);
        #endregion

        private static readonly IntPtr HwndTop = IntPtr.Zero;
        private const int SwpShowwindow = 64; // 0×0040

        public static void SetWinFullScreen(IntPtr hwnd, int left, int top, int width, int height)
        {
            SetWindowPos(hwnd, HwndTop, left, top, width, height, SwpShowwindow);
        }
    }
}
