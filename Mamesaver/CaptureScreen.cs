using System;
using System.Drawing;
using Mamesaver.Windows;
using Serilog;

namespace Mamesaver
{
    public class CaptureScreen : IDisposable
    {
        private Rectangle _sourceRect;
        private IntPtr _sourceHwnd;
        private IntPtr _hDeviceContext;
        private IntPtr _hMemoryContext = IntPtr.Zero;
        private IntPtr _hBitmap = IntPtr.Zero;

        public void Initialise(Rectangle sourceRect)
        {
            _sourceRect = sourceRect;
            _sourceHwnd = PlatformInvokeUser32.GetDesktopWindow();
            _hDeviceContext = PlatformInvokeUser32.GetDC(_sourceHwnd);
        }

        public void CloneTo(IntPtr destinationDeviceContext, Rectangle destinationRect)
        {
            PlatformInvokeGdi32.StretchBlt(destinationDeviceContext, 0, 0, destinationRect.Width, destinationRect.Height,
                _hDeviceContext, _sourceRect.X, _sourceRect.Y, _sourceRect.Width, _sourceRect.Height, PlatformInvokeGdi32.SRCOPY);
        }

        public void Dispose()
        {
            Log.Debug("{class} Dispose()", GetType().Name);

            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~CaptureScreen()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            try
            {
                if (_hBitmap != IntPtr.Zero)
                {
                    PlatformInvokeGdi32.DeleteObject(_hBitmap);
                    _hBitmap = IntPtr.Zero;
                }

                if (_hMemoryContext != IntPtr.Zero)
                {
                    PlatformInvokeGdi32.DeleteDC(_hMemoryContext);
                    _hMemoryContext = IntPtr.Zero;
                }

                if (_hDeviceContext != IntPtr.Zero)
                {
                    PlatformInvokeUser32.ReleaseDC(_sourceHwnd, _hDeviceContext);
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