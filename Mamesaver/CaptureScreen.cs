using System;
using System.Drawing;
using Serilog;
using static Mamesaver.Windows.PlatformInvokeUser32;
using static Mamesaver.Windows.PlatformInvokeGdi32;

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
            _sourceHwnd = GetDesktopWindow();
            _hDeviceContext = GetDC(_sourceHwnd);
        }

        public void CloneTo(IntPtr destinationDeviceContext, Rectangle destinationRect)
        {
            StretchBlt(destinationDeviceContext, 0, 0, destinationRect.Width, destinationRect.Height,
                _hDeviceContext, _sourceRect.X, _sourceRect.Y, _sourceRect.Width, _sourceRect.Height, SRCOPY);
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