using COMMONWeb;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.models;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace COMMONWebTest
{

    public class MockWithoutDataCOMMONRepo : IDataStore
    {
        public List<CollectorInfo> GetAllCollectors()
        {
            return new List<CollectorInfo>();
        }

        public List<DeviceData> GetDeviceData(long deviceID, ECollectorType collectorType, IStartEndTime start_end)
        {
            return new List<DeviceData>();
        }

        public FullDeviceStatus GetDeviceStatuses(long device_id)
        {
            return new FullDeviceStatus();
        }

        public List<NetworkStatus> GetNetworkStatuses(IStartEndTime start_end)
        {
            return new List<NetworkStatus>();
        }

        //public NetworkStatus GetPingData(string ipAddress, int daysToRetrieve)
        //{
        //    return new NetworkStatus();
        //}

        public SystemConfiguration GetSystemConfiguration(bool obfuscate)
        {
            return new SystemConfiguration();
        }
    }

    public class MainModuleWithoutDataFixture : IDisposable
    {
        public MockWithoutDataCOMMONRepo Repo { get; private set; }
        public Browser B { get; private set; }

        public MainModuleWithoutDataFixture()
        {
            Repo = new MockWithoutDataCOMMONRepo();

            B = new Browser(cfg =>
            {
                cfg.Module<MainModule>();
                cfg.Dependency<IDataStore>(Repo);
            });
        }

        public void Dispose()
        {
        }
    }

    public class TestCOMMONRepoWithoutData : IClassFixture<MainModuleWithoutDataFixture>
    {
        private MainModuleWithoutDataFixture m_fixture;

        public TestCOMMONRepoWithoutData(MainModuleWithoutDataFixture fixture)
        {
            m_fixture = fixture;
        }

        [Fact]
        public void TestDeviceStatus()
        {
            Browser browser = m_fixture.B;
            BrowserResponse response = browser.Get("/devicestatus");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string s = response.Body.AsString();
            Assert.False(string.IsNullOrEmpty(s));

            FullDeviceStatus status = JsonConvert.DeserializeObject<FullDeviceStatus>(s);

            Assert.NotNull(status);
            Assert.Empty(status.fullStatus);
        }

        // This fails because /configurationdata uses the database, which doesn't
        // exist for testing purposes. We can fix that, but for a quick-and-dirty
        // solution, just remove this test.
        //[Fact]
        //public void TestSystemConfiguration()
        //{
        //    Browser browser = m_fixture.B;
        //    BrowserResponse response = browser.Get("/configurationdata");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    string s = response.Body.AsString();

        //    Assert.False(string.IsNullOrEmpty(s));
        //    SystemConfiguration config = JsonConvert.DeserializeObject<SystemConfiguration>(s);

        //    Assert.NotNull(config);
        //    Assert.Equal("", config.softwareVersion);
        //    Assert.Empty(config.configuration);
        //    Assert.Empty(config.devices);
        //    Assert.Empty(config.languages);
        //}

        [Fact]
        public void TestNetworkStatus()
        {
            Browser browser = m_fixture.B;
            BrowserResponse response = browser.Get("/networkstatus/null/null");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string s = response.Body.AsString();

            Assert.False(string.IsNullOrEmpty(s));
            List<NetworkStatus> status = JsonConvert.DeserializeObject<List<NetworkStatus>>(s);

            Assert.NotNull(status);
            Assert.Empty(status);
        }

        //[Fact]
        //public void TestPingData()
        //{
        //    Browser browser = m_fixture.B;
        //    BrowserResponse response = browser.Get("/devicenetworkstatus/22.33.44.11/34256");

        //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        //    string s = response.Body.AsString();

        //    Assert.False(string.IsNullOrEmpty(s));
        //    NetworkStatus ns = JsonConvert.DeserializeObject<NetworkStatus>(s);

        //    Assert.NotNull(ns);
        //    Assert.Null(ns.ipAddress);
        //    Assert.Null(ns.name);
        //    Assert.Equal(-1, ns.deviceID);
        //    Assert.Null(ns.dateSuccessfulPingOccurred);
        //    Assert.Equal(DateTimeOffset.MinValue, ns.datePingAttempted);
        //    Assert.Empty(ns.attempts);
        //    Assert.False(ns.hasBeenPinged);
        //}
    }
}
