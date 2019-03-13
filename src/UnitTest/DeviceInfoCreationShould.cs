using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.models;
using System.Collections.Generic;
using Xunit;

namespace UnitTest
{
    public class DeviceInfoCreationShould
    {
        [Fact]
        public void CreateSystemCorrectly()
        {
            DeviceInfo di = new DeviceInfo(EDeviceType.System) { name = "System" };
            Assert.Collection(di.collectors,
                c => Assert.Equal(ECollectorType.Ping, c.collectorType),
                c => Assert.Equal(ECollectorType.Configuration, c.collectorType));
        }

        [Fact]
        public void CreateServerCorrectly()
        {
            DeviceInfo server = new DeviceInfo(EDeviceType.Server) { name = "Server" };
            Assert.True(server.collectors.Count > 0);

            List<ECollectorType> collectors = new List<ECollectorType>(new ECollectorType[] {
                ECollectorType.Memory,
                ECollectorType.Disk,
                ECollectorType.CPUUsage,
                ECollectorType.NICUsage,
                ECollectorType.Uptime,
                ECollectorType.LastBootTime,
                ECollectorType.Processes,
                ECollectorType.InstalledApplications,
                ECollectorType.Services,
                ECollectorType.SystemErrors,
                ECollectorType.ApplicationErrors,
                ECollectorType.DatabaseSize,
                ECollectorType.UPS,
                ECollectorType.DiskSpeed,
                ECollectorType.SMART,
                //CollectorType.AntiVirus,
                //CollectorType.Firewall,
            });

            collectors.ForEach(c => Assert.NotNull(server.collectors.Find(sc => sc.collectorType == c)));
        }

        [Fact]
        public void CreateWorkstationCorrectly()
        {
            DeviceInfo workstation = new DeviceInfo(EDeviceType.Server) { name = "Workstation" };
            Assert.True(workstation.collectors.Count > 0);

            List<ECollectorType> collectors = new List<ECollectorType>(new ECollectorType[] {
                ECollectorType.Memory,
                ECollectorType.Disk,
                ECollectorType.CPUUsage,
                ECollectorType.NICUsage,
                ECollectorType.Uptime,
                ECollectorType.LastBootTime,
                ECollectorType.Processes,
                ECollectorType.InstalledApplications,
                ECollectorType.Services,
                ECollectorType.SystemErrors,
                ECollectorType.ApplicationErrors,
                ECollectorType.DatabaseSize,
                ECollectorType.UPS,
                ECollectorType.DiskSpeed,
                ECollectorType.SMART,
                //CollectorType.AntiVirus,
                //CollectorType.Firewall,
            });

            collectors.ForEach(c => Assert.NotNull(workstation.collectors.Find(sc => sc.collectorType == c)));
        }
    }
}
