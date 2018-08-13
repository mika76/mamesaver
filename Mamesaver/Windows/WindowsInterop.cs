using System;

namespace Mamesaver.Windows
{
    public static class WindowsInterop
    {
        private static readonly IntPtr HwndTop = IntPtr.Zero;
        private const int SwpShowwindow = 64; // 0×0040

        public static void SetWinFullScreen(IntPtr hwnd, int left, int top, int width, int height)
        {
            PlatformInvokeUser32.SetWindowPos(hwnd, HwndTop, left, top, width, height, SwpShowwindow);
        }
    }
}
