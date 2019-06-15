using Mamesaver.Services.Windows;
using NUnit.Framework;

namespace Mamesaver.Test.Integration
{
    [TestFixture]
    public class PowerTests
    {
        [Test]
        [Description("Sanity check for retrieving power policy and type")]
        public void GetPowerPolicy()
        {
            var powerType = PowerInterop.GetPowerType();
            Assert.NotNull(powerType);

            var policy = PowerInterop.GetPowerPolicy(powerType.Value);
            Assert.NotNull(policy);
        }
    }
}
