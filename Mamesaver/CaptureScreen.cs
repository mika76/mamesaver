using System;
using System.Drawing;
using Serilog;
using static Mamesaver.Windows.PlatformInvokeUser32;
using static Mamesaver.Windows.PlatformInvokeGdi32;

namespace Mamesaver
{
    public class CaptureScreen : IDisposable
    {
        private readonly IntPtr _sourceHwnd;
        private readonly int _width;
        private readonly int _height;
        private IntPtr _hDeviceContext;
        private IntPtr _hMemoryContext = IntPtr.Zero;
        private IntPtr _hBitmap = IntPtr.Zero;

        public CaptureScreen() : this(
            GetDesktopWindow(),
            GetSystemMetrics(SM_CXSCREEN),
            GetSystemMetrics(SM_CYSCREEN))
        {
        }

        private CaptureScreen(IntPtr sourceHwnd, int width, int height)
        {
            _sourceHwnd = sourceHwnd;
            _width = width;
            _height = height;

            _hDeviceContext = GetDC(_sourceHwnd);
        }

        public Bitmap Capture()
        {
            if (_hMemoryContext == IntPtr.Zero) _hMemoryContext = CreateCompatibleDC(_hDeviceContext);
            if (_hBitmap == IntPtr.Zero) _hBitmap = CreateCompatibleBitmap(_hDeviceContext, _width, _height);

            SelectObject(_hMemoryContext, _hBitmap);
            BitBlt(_hMemoryContext, 0, 0, _width, _height, _hDeviceContext, 0, 0, SRCOPY);
            var bitmap = Image.FromHbitmap(_hBitmap);
            return bitmap;
        }

        public void CloneTo(IntPtr destinationDeviceContext, Rectangle destinationRect)
        {
            Log.Verbose("Cloning screen {width}x{height} to {destination}", _width, _height, destinationRect);

            StretchBlt(destinationDeviceContext, 0, 0, destinationRect.Width, destinationRect.Height,
                _hDeviceContext, 0, 0, _width, _height, SRCOPY);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing) => ReleaseUnmanagedResources();
        ~CaptureScreen() => Dispose(false);

        private void ReleaseUnmanagedResources()
        {
            try
            {
                if (_hBitmap != IntPtr.Zero)
                {
                    DeleteObject(_hBitmap);
                    _hBitmap = IntPtr.Zero;
                }

                if (_hMemoryContext != IntPtr.Zero)
                {
                    DeleteDC(_hMemoryContext);
                    _hMemoryContext = IntPtr.Zero;
                }

                if (_hDeviceContext != IntPtr.Zero)
                {
                    ReleaseDC(_sourceHwnd, _hDeviceContext);
                    _hDeviceContext = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error releasing unmanaged resources");
            }
        }
    }
}