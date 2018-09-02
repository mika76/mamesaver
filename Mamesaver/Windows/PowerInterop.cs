using System;
using Serilog;

namespace Mamesaver.Windows
{
    public static class PowerInterop
    {
        public static PowerType? GetPowerType()
        {
            var powerState = new SYSTEM_POWER_STATUS();
            if (!PlatformInvokeKernel32.GetSystemPowerStatusRef(powerState))
            {
                Log.Warning("Unable to get power state");
                return null;
            }

            return powerState.ACLineStatus == ACLineStatus.Online ? PowerType.AC : PowerType.DC;
        }

        public static PowerPolicy GetPowerPolicy(PowerType powerType)
        {
            var powerPolicy = new POWER_POLICY();
            var globalPowerPolicy = new GLOBAL_POWER_POLICY();

            if (!PlatformInvokerPowrprof.GetCurrentPowerPolicies(ref globalPowerPolicy, ref powerPolicy))
            {
                Log.Warning("Unable to get power management state");
                return null;
            }

            var userPolicy = powerPolicy.user;

            switch (powerType)
            {
                case PowerType.AC:
                    return new PowerPolicy
                    {
                        IdleTimeout =
                            userPolicy.IdleTimeoutAc == 0 || userPolicy.IdleAc.Action == POWER_ACTION.PowerActionNone
                                ? TimeSpan.MaxValue
                                : TimeSpan.FromSeconds(userPolicy.IdleTimeoutAc),
                        VideoTimeout = userPolicy.VideoTimeoutAc == 0
                            ? TimeSpan.MaxValue
                            : TimeSpan.FromSeconds(userPolicy.VideoTimeoutAc)
                    };
                case PowerType.DC:
                    return new PowerPolicy
                    {
                        IdleTimeout =
                            userPolicy.IdleTimeoutDc == 0 || userPolicy.IdleDc.Action == POWER_ACTION.PowerActionNone
                                ? TimeSpan.MaxValue
                                : TimeSpan.FromSeconds(userPolicy.IdleTimeoutDc),
                        VideoTimeout = userPolicy.VideoTimeoutDc == 0
                            ? TimeSpan.MaxValue
                            : TimeSpan.FromSeconds(userPolicy.VideoTimeoutDc)
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerType));
            }
        }
    }

    public enum PowerType
    {
        AC,
        DC
    }

    public class PowerPolicy
    {
        public TimeSpan VideoTimeout { get; set; }
        public TimeSpan IdleTimeout { get; set; }
    }
}