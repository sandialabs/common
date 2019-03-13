using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTest
{
    public class EDeviceTypeShould
    {
        [Fact]
        public void TestAllValues()
        {
            List<EDeviceType> types = new List<EDeviceType>(EnumExtensions.GetValues<EDeviceType>());
            types.Remove(EDeviceType.Server);
            types.Remove(EDeviceType.Workstation);
            types.Remove(EDeviceType.Camera);
            types.Remove(EDeviceType.RPM);
            types.Remove(EDeviceType.System);
            types.Remove(EDeviceType.Generic);
            types.Remove(EDeviceType.Unknown);

            // If something else is added at some point, this will fail
            Assert.Empty(types);
        }

        [Fact]
        public void ReportWindowsMachineProperly()
        {
            Assert.True(EDeviceType.Server.IsWindowsMachine());
            Assert.True(EDeviceType.Workstation.IsWindowsMachine());
            Assert.False(EDeviceType.Camera.IsWindowsMachine());
            Assert.False(EDeviceType.Generic.IsWindowsMachine());
            Assert.False(EDeviceType.RPM.IsWindowsMachine());
            Assert.False(EDeviceType.System.IsWindowsMachine());
            Assert.False(EDeviceType.Unknown.IsWindowsMachine());
        }
    }
}
