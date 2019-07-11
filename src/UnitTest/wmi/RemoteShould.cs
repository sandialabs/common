using gov.sandia.sld.common.data.wmi;
using System.Management;
using Xunit;

namespace UnitTest.wmi
{
    public class RemoteShould
    {
        [Fact]
        public void ConstructOK()
        {
            Remote r = new Remote();

            Assert.Equal(string.Empty, r.IPAddress);
            Assert.Equal(string.Empty, r.Username);
            Assert.Equal(string.Empty, r.Password);
            Assert.False(r.HasIPAddress);
            Assert.False(r.HasUsernamePassword);

            r = new Remote("1.2.3.4");

            Assert.Equal("1.2.3.4", r.IPAddress);
            Assert.Equal(string.Empty, r.Username);
            Assert.Equal(string.Empty, r.Password);
            Assert.True(r.HasIPAddress);
            Assert.False(r.HasUsernamePassword);

            r = new Remote("0.0.0.0");

            Assert.Equal("0.0.0.0", r.IPAddress);
            Assert.Equal(string.Empty, r.Username);
            Assert.Equal(string.Empty, r.Password);
            Assert.False(r.HasIPAddress);
            Assert.False(r.HasUsernamePassword);

            r = new Remote("localhost");

            Assert.Equal("localhost", r.IPAddress);
            Assert.Equal(string.Empty, r.Username);
            Assert.Equal(string.Empty, r.Password);
            Assert.False(r.HasIPAddress);
            Assert.False(r.HasUsernamePassword);

            r = new Remote("bogus");

            Assert.Equal("bogus", r.IPAddress);
            Assert.Equal(string.Empty, r.Username);
            Assert.Equal(string.Empty, r.Password);
            Assert.False(r.HasIPAddress);
            Assert.False(r.HasUsernamePassword);

            r = new Remote("1.2.3.4", "abc", "pwd123");

            Assert.Equal("1.2.3.4", r.IPAddress);
            Assert.Equal("abc", r.Username);
            Assert.Equal("pwd123", r.Password);
            Assert.True(r.HasIPAddress);
            Assert.True(r.HasUsernamePassword);
        }

        [Fact]
        public void GeneratesManagementScopeProperly()
        {
            Remote r = new Remote();
            ManagementScope ms = r.GetManagementScope("abc");

            Assert.NotNull(ms);
            Assert.Equal(@"\\.\root\abc", ms.Path.Path);

            // Don't specify a username/password because it automatically attempts
            // to connect using the specified credentials, but this IP obviously
            // doesn't exist. That's another test.
            r = new Remote("1.2.3.4");
            ms = r.GetManagementScope("abc");

            Assert.NotNull(ms);
            Assert.Equal(@"\\1.2.3.4\root\abc", ms.Path.Path);
        }
    }
}
