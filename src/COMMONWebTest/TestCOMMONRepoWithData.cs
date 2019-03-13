using Xunit;
using Nancy.Testing;
using COMMONWeb;
using gov.sandia.sld.common.db;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using gov.sandia.sld.common.configuration;
using Nancy;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.models;

namespace COMMONWebTest
{
    public class MockWithDataCOMMONRepo : IDataStore
    {
        public List<CollectorInfo> GetAllCollectors()
        {
            List<CollectorInfo> collectors = new List<CollectorInfo>();
            CollectorInfo info = new CollectorInfo(ECollectorType.CPUUsage);
            info.CID = new CollectorID(1, "Device1.CPU");
            info.DID = new DeviceID(1, "Device1");
            collectors.Add(info);
            return collectors;
        }

        public List<DeviceData> GetDeviceData(long deviceID, ECollectorType collectorType, IStartEndTime start_end)
        {
            List<DeviceData> device_data = new List<DeviceData>();
            device_data.Add(new DeviceData() { collectorID = 2233, dataID = 3344, timeStamp = DateTimeOffset.Now.AddHours(-6), value = "{}" });
            return device_data;
        }

        public FullDeviceStatus GetDeviceStatuses(long device_id)
        {
            FullDeviceStatus fds = new FullDeviceStatus();
            if (device_id < 0)
            {
                fds.AddStatus("A", 2345, "Description-A1", EAlertLevel.Normal, "Message-A1");
                fds.AddStatus("A", 2345, "Description-A2", EAlertLevel.Alarm, "Message-A2");
                fds.AddStatus("B", 3344, "Description-B1", EAlertLevel.Information, "Message-B1");
            }
            else
            {
                fds.AddStatus("A", device_id, "Description-A1", EAlertLevel.Normal, "Message-A1");
                fds.AddStatus("A", device_id, "Description-A2", EAlertLevel.Alarm, "Message-A2");
                fds.AddStatus("B", device_id, "Description-B1", EAlertLevel.Information, "Message-B1");
            }
            return fds;
        }

        public List<NetworkStatus> GetNetworkStatuses(IStartEndTime start_end)
        {
            List<NetworkStatus> statuses = new List<NetworkStatus>();
            DateTimeOffset dt = new DateTimeOffset(2017, 10, 23, 11, 42, 33, TimeSpan.FromHours(-6));
            statuses.Add(new NetworkStatus()
            {
                name = "Status A1",
                deviceID = 2,
                successfulPing = true,
                dateSuccessfulPingOccurred = dt,
                datePingAttempted = dt,
                ipAddress = "1.2.3.4",
                hasBeenPinged = true,
            });
            statuses.Add(new NetworkStatus()
            {
                name = "Status A2",
                deviceID = 15,
                successfulPing = false,
                dateSuccessfulPingOccurred = dt,
                datePingAttempted = dt,
                ipAddress = "2.3.4.5",
                hasBeenPinged = true,
            });
            statuses.Add(new NetworkStatus()
            {
                name = "Status A3",
                deviceID = 11,
                successfulPing = false,
                dateSuccessfulPingOccurred = null,
                datePingAttempted = dt,
                ipAddress = "3.4.5.6",
                hasBeenPinged = false,
            });

            return statuses;
        }

        //public NetworkStatus GetPingData(string ipAddress, int daysToRetrieve)
        //{
        //    DateTimeOffset dt = new DateTimeOffset(2017, 10, 23, 11, 42, 33, TimeSpan.FromHours(-6));
        //    NetworkStatus status = new NetworkStatus()
        //    {
        //        ipAddress = ipAddress,
        //        deviceID = 11,
        //        successfulPing = true,
        //        dateSuccessfulPingOccurred = dt,
        //        datePingAttempted = dt,
        //        hasBeenPinged = true,
        //        name = ipAddress
        //    };
        //    status.attempts.Add(new PingAttempt()
        //    {
        //        successful = true,
        //        timestamp = dt - TimeSpan.FromHours(3)
        //    });
        //    status.attempts.Add(new PingAttempt()
        //    {
        //        successful = false,
        //        timestamp = dt - TimeSpan.FromHours(2)
        //    });
        //    status.attempts.Add(new PingAttempt()
        //    {
        //        successful = true,
        //        timestamp = dt - TimeSpan.FromHours(1)
        //    });
        //    return status;
        //}

        public SystemConfiguration GetSystemConfiguration(bool obfuscate)
        {
            SystemConfiguration config = new SystemConfiguration();
            config.configuration.Add("config.test", new ConfigurationData()
            {
                configID = 1,
                deleted = false,
                path = "config.test",
                value = "Test configuration"
            });

            DeviceInfo info = new DeviceInfo()
            {
                DID = new DeviceID(234, "Test Device"),
                deleted = false,
                ipAddress = "2.3.4.5",
                username = "mysupersecretusername",
                password = "mysupersecretpassword",
                type = EDeviceType.System
            };
            info.collectors.Add(new CollectorInfo(ECollectorType.Disk));
            config.devices.Add(info);

            info = new DeviceInfo()
            {
                DID = new DeviceID(345, "Test Device 2"),
                deleted = true,
                ipAddress = "3.4.5.6",
                username = "dave",
                password = "davepass",
                type = EDeviceType.Server
            };
            info.collectors.Add(new CollectorInfo(ECollectorType.Memory));
            config.devices.Add(info);

            config.languages.Add(new LanguageConfiguration()
            {
                languageCode = "en",
                language = "English",
                isEnabled = true
            });
            config.languages.Add(new LanguageConfiguration()
            {
                languageCode = "ro",
                language = "Romanian",
                isEnabled = false
            });

            config.softwareVersion = "2.3.4";

            return config;
        }
    }

    public class MainModuleWithDataFixture : IDisposable
    {
        public MockWithDataCOMMONRepo Repo { get; private set; }
        public Browser B { get; private set; }

        public MainModuleWithDataFixture()
        {
            Repo = new MockWithDataCOMMONRepo();

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

    public class TestCOMMONRepoWithData : IClassFixture<MainModuleWithDataFixture>
    {
        private MainModuleWithDataFixture m_fixture;

        public TestCOMMONRepoWithData(MainModuleWithDataFixture fixture)
        {
            m_fixture = fixture;
        }

        [Fact]
        public void TestRoot()
        {
            Browser browser = m_fixture.B;
            BrowserResponse response = browser.Get("");

            string s = response.Body.AsString();
            Assert.Contains("ng-app", s);
        }

        [Fact]
        public void TestBadURLs()
        {
            Browser browser = m_fixture.B;
            BrowserResponse response = browser.Get("/bogusurl");

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // Valid method (processhistory), but invalid params (the
            // first param should be an integer).
            response = browser.Get("/processhistory/x/2");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
            Assert.Equal(2, status.fullStatus.Count);

            List<DeviceStatus> lds = status.fullStatus[2345];
            Assert.Equal(2, lds.Count);
            DeviceStatus ds = lds[0];
            Assert.Contains("-A1", ds.status);
            Assert.Contains("-A1", ds.message);
            Assert.Equal((int)EAlertLevel.Normal, ds.alertLevel);
            ds = lds[1];
            Assert.Contains("-A2", ds.status);
            Assert.Contains("-A2", ds.message);
            Assert.Equal((int)EAlertLevel.Alarm, ds.alertLevel);

            lds = status.fullStatus[3344];
            Assert.Single(lds);
            ds = lds[0];
            Assert.Contains("-B1", ds.status);
            Assert.Contains("-B1", ds.message);
            Assert.Equal((int)EAlertLevel.Information, ds.alertLevel);
        }

        //  For now, the web site always uses the actual database, which fails when we
        //  are doing the testing, so let's just comment this out until that can be fixed.

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
        //    Assert.Equal("2.3.4", config.softwareVersion);

        //    Assert.Single(config.configuration);
        //    ConfigurationData config_data = config.configuration["config.test"];
        //    Assert.NotNull(config_data);
        //    Assert.Equal(1, config_data.configID);
        //    Assert.False(config_data.deleted);
        //    Assert.Equal("config.test", config_data.path);
        //    Assert.Equal("Test configuration", config_data.value);

        //    Assert.Equal(2, config.devices.Count);
        //    DeviceInfo info = config.devices[0];
        //    Assert.NotNull(info);
        //    Assert.Equal(234, info.id);
        //    Assert.False(info.deleted);
        //    Assert.Equal("2.3.4.5", info.ipAddress);
        //    Assert.Equal("Test Device", info.name);
        //    // These should be empty because the configuration is returned obfuscated
        //    Assert.Empty(info.username);
        //    Assert.Empty(info.password);
        //    Assert.Equal(DeviceType.System, info.type);
        //    Assert.Single(info.collectors);
        //    CollectorInfo coll_info = info.collectors[0];
        //    Assert.NotNull(coll_info);
        //    Assert.Equal(CollectorType.Disk, coll_info.collectorType);

        //    info = config.devices[1];
        //    Assert.NotNull(info);
        //    Assert.Equal(345, info.id);
        //    Assert.True(info.deleted);
        //    Assert.Equal("3.4.5.6", info.ipAddress);
        //    Assert.Equal("Test Device 2", info.name);
        //    // These should be empty because the configuration is returned obfuscated
        //    Assert.Empty(info.username);
        //    Assert.Empty(info.password);
        //    Assert.Equal(DeviceType.Server, info.type);
        //    Assert.Single(info.collectors);
        //    coll_info = info.collectors[0];
        //    Assert.NotNull(coll_info);
        //    Assert.Equal(CollectorType.Memory, coll_info.collectorType);

        //    Assert.Equal(2, config.languages.Count);
        //    LanguageConfiguration lang = config.languages[0];
        //    Assert.NotNull(lang);
        //    Assert.Equal("en", lang.languageCode);
        //    Assert.Equal("English", lang.language);
        //    Assert.True(lang.isEnabled);
        //    lang = config.languages[1];
        //    Assert.NotNull(lang);
        //    Assert.Equal("ro", lang.languageCode);
        //    Assert.Equal("Romanian", lang.language);
        //    Assert.False(lang.isEnabled);
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
            Assert.Equal(3, status.Count);

            NetworkStatus ns = status[0];
            Assert.NotNull(ns);
            Assert.Equal("Status A1", ns.name);
            Assert.Equal(2, ns.deviceID);
            Assert.True(ns.successfulPing);
            Assert.NotNull(ns.dateSuccessfulPingOccurred);
            Assert.Equal(23, ns.dateSuccessfulPingOccurred.Value.Day);
            Assert.Equal(23, ns.datePingAttempted.Day);
            Assert.Equal("1.2.3.4", ns.ipAddress);
            Assert.True(ns.hasBeenPinged);
            Assert.Empty(ns.attempts);

            ns = status[1];
            Assert.NotNull(ns);
            Assert.Equal("Status A2", ns.name);
            Assert.Equal(15, ns.deviceID);
            Assert.False(ns.successfulPing);
            Assert.NotNull(ns.dateSuccessfulPingOccurred);
            Assert.Equal(23, ns.dateSuccessfulPingOccurred.Value.Day);
            Assert.Equal(23, ns.datePingAttempted.Day);
            Assert.Equal("2.3.4.5", ns.ipAddress);
            Assert.True(ns.hasBeenPinged);
            Assert.Empty(ns.attempts);

            ns = status[2];
            Assert.NotNull(ns);
            Assert.Equal("Status A3", ns.name);
            Assert.Equal(11, ns.deviceID);
            Assert.False(ns.successfulPing);
            Assert.Null(ns.dateSuccessfulPingOccurred);
            Assert.Equal(23, ns.datePingAttempted.Day);
            Assert.Equal("3.4.5.6", ns.ipAddress);
            Assert.False(ns.hasBeenPinged);
            Assert.Empty(ns.attempts);
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
        //    Assert.Equal("22.33.44.11", ns.ipAddress);
        //    Assert.Equal("22.33.44.11", ns.name);
        //    Assert.Equal(11, ns.deviceID);
        //    Assert.NotNull(ns.dateSuccessfulPingOccurred);
        //    Assert.Equal(23, ns.dateSuccessfulPingOccurred.Value.Day);
        //    Assert.Equal(23, ns.datePingAttempted.Day);
        //    Assert.True(ns.hasBeenPinged);
        //    Assert.Equal(3, ns.attempts.Count);

        //    PingAttempt pa = ns.attempts[0];
        //    Assert.True(pa.successful);
        //    Assert.Equal(8, pa.timestamp.Hour);
        //    pa = ns.attempts[1];
        //    Assert.False(pa.successful);
        //    Assert.Equal(9, pa.timestamp.Hour);
        //    pa = ns.attempts[2];
        //    Assert.True(pa.successful);
        //    Assert.Equal(10, pa.timestamp.Hour);
        //}
    }
}