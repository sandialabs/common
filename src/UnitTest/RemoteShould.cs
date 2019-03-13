using Xunit;
using gov.sandia.sld.common.data;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using gov.sandia.sld.common.data.wmi;

namespace UnitTest
{
    public class RemoteShould
    {
        [Fact]
        public void Remote()
        {
            Remote remote = new Remote()
            {
                IPAddress = "192.168.0.1",
                Username = "test",
                Password = "password"
            };

            Assert.Equal("192.168.0.1", remote.IPAddress);
            Assert.Equal("test", remote.Username);
            Assert.Equal("password", remote.Password);
        }
    }
}
