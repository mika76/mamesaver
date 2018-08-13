using System;
using System.Drawing;
using Mamesaver.Windows;

namespace Mamesaver
{
    public class CaptureScreen
    {
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        public static Bitmap GetDesktopImage()
        {
            return Capture(PlatformInvokeUser32.GetDesktopWindow(),
                PlatformInvokeUser32.GetSystemMetrics(PlatformInvokeUser32.SM_CXSCREEN),
                PlatformInvokeUser32.GetSystemMetrics(PlatformInvokeUser32.SM_CYSCREEN));
        }

        public static Bitmap Capture(IntPtr hwnd, int width, int height)
        {
            var hDeviceContext = PlatformInvokeUser32.GetDC(hwnd);
            var hMemoryContext = PlatformInvokeGdi32.CreateCompatibleDC(hDeviceContext);

            SIZE size;
            size.cx = width;
            size.cy = height;

            var hBitmap = PlatformInvokeGdi32.CreateCompatibleBitmap(hDeviceContext, size.cx, size.cy);
            if (hBitmap == IntPtr.Zero) return null;

            var hOld = PlatformInvokeGdi32.SelectObject(hMemoryContext, hBitmap);
            PlatformInvokeGdi32.BitBlt(hMemoryContext, 0, 0, size.cx, size.cy, hDeviceContext, 0, 0, PlatformInvokeGdi32.SRCOPY);
            PlatformInvokeGdi32.SelectObject(hMemoryContext, hOld);
            PlatformInvokeGdi32.DeleteDC(hMemoryContext);
            PlatformInvokeUser32.ReleaseDC(hwnd, hDeviceContext);

            var bitmap = Image.FromHbitmap(hBitmap);
            PlatformInvokeGdi32.DeleteObject(hBitmap);
            GC.Collect();

            return bitmap;
        }
    }
}
