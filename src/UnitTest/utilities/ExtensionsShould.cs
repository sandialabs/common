using gov.sandia.sld.common.utilities;
using Xunit;

namespace UnitTest.utilities
{
    public class ExtensionsShould
    {
        [Fact]
        public void IsIPAddressWorksProperly()
        {
            string ip = string.Empty;

            Assert.False(ip.IsIPAddress());

            ip = null;

            Assert.False(ip.IsIPAddress());

            ip = "0.0.0.0";

            Assert.False(ip.IsIPAddress());

            ip = "localhost";

            Assert.False(ip.IsIPAddress());

            ip = "whatever";

            Assert.False(ip.IsIPAddress());

            ip = "1.2.3.4";

            Assert.True(ip.IsIPAddress());
        }
    }
}
