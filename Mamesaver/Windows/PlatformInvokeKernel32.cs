using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Mamesaver.Windows
{
    internal static class PlatformInvokeKernel32
    {
        [DllImport("Kernel32", EntryPoint = "GetSystemPowerStatus")]
        public static extern bool GetSystemPowerStatusRef(SYSTEM_POWER_STATUS sps);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class SYSTEM_POWER_STATUS
    {
        public ACLineStatus ACLineStatus;
        public BatteryFlag BatteryFlag;
        public byte BatteryLifePercent;
        public byte Reserved1;
        public int BatteryLifeTime;
        public int BatteryFullLifeTime;
    }

    // Note: Underlying type of byte to match Win32 header
    public enum ACLineStatus : byte
    {
        Offline = 0, Online = 1, Unknown = 255
    }

    public enum BatteryFlag : byte
    {
        High = 1, Low = 2, Critical = 4, Charging = 8,
        NoSystemBattery = 128, Unknown = 255
    }
}