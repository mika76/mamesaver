using System;
using System.Drawing;
using Mamesaver.Windows;

namespace Mamesaver
{
    public class CaptureScreen : IDisposable
    {
        private readonly IntPtr _sourceHwnd;
        private readonly int _width;
        private readonly int _height;
        private readonly IntPtr _hDeviceContext;
        private readonly IntPtr _hMemoryContext;
        private readonly IntPtr _hBitmap;

        public CaptureScreen() : this(
            PlatformInvokeUser32.GetDesktopWindow(),
            PlatformInvokeUser32.GetSystemMetrics(PlatformInvokeUser32.SM_CXSCREEN),
            PlatformInvokeUser32.GetSystemMetrics(PlatformInvokeUser32.SM_CYSCREEN))
        {
        }

        public CaptureScreen(IntPtr sourceHwnd, int width, int height)
        {
            _sourceHwnd = sourceHwnd;
            _width = width;
            _height = height;

            _hDeviceContext = PlatformInvokeUser32.GetDC(_sourceHwnd);
            _hMemoryContext = PlatformInvokeGdi32.CreateCompatibleDC(_hDeviceContext);
            _hBitmap = PlatformInvokeGdi32.CreateCompatibleBitmap(_hDeviceContext, _width, _height);
            PlatformInvokeGdi32.SelectObject(_hMemoryContext, _hBitmap);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~CaptureScreen()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            PlatformInvokeGdi32.DeleteObject(_hBitmap);
            PlatformInvokeGdi32.DeleteDC(_hMemoryContext);
            PlatformInvokeUser32.ReleaseDC(_sourceHwnd, _hDeviceContext);
        }

        public Bitmap Capture()
        {
            PlatformInvokeGdi32.BitBlt(_hMemoryContext, 0, 0, _width, _height, _hDeviceContext, 0, 0, PlatformInvokeGdi32.SRCOPY);
            var bitmap = Image.FromHbitmap(_hBitmap);
            return bitmap;
        }
    }
}
