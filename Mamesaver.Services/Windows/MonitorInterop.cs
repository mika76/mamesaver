using System;

// ReSharper disable InconsistentNaming
namespace Mamesaver.Services.Windows
{
    public static class MonitorInterop
    {
        private const int WM_SYSCOMMAND = 0x112;
        private const int SC_MONITORPOWER = 0xF170;

        public static void SetMonitorState(IntPtr handle, MonitorState state)
        {
            PlatformInvokeUser32.SendMessage(handle, WM_SYSCOMMAND, SC_MONITORPOWER, new IntPtr((int)state));
        }
    }

    public enum MonitorState
    {
        MonitorStateOn = -1,
        MonitorStateOff = 2,
        MonitorStateStandBy = 1
    }
}