using gov.sandia.sld.common.configuration;
using System;
using Xunit;

namespace UnitTest.statustype
{
    public class DiskSpaceStatusShould
    {
        [Fact]
        public void GenerateStatusTypesProperly()
        {
            DiskSpaceStatus ds_status = new DiskSpaceStatus(80.0, 90.0);

            Tuple<EStatusType, double> status = ds_status.GetStatus(200, 1000);
            Assert.Equal(EStatusType.AdequateDiskSpace, status.Item1);

            status = ds_status.GetStatus(800, 1000);
            Assert.Equal(EStatusType.LowOnDiskSpace, status.Item1);

            status = ds_status.GetStatus(900, 1000);
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.Item1);

            status = ds_status.GetStatus(999, 1000);
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.Item1);

            status = ds_status.GetStatus(0, 1000);
            Assert.Equal(EStatusType.AdequateDiskSpace, status.Item1);
        }

        [Fact]
        public void HandleLargeNumbersOK()
        {
            DiskSpaceStatus ds_status = new DiskSpaceStatus(80.0, 90.0);

            // Handle large #s OK
            Tuple<EStatusType, double> status = ds_status.GetStatus(10_000_000_000_000, 10_500_000_000_000);
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.Item1);
            Assert.Equal(95.2, status.Item2, 1);
        }

        [Fact]
        public void HandleMaxThresholdsProperly()
        {
            DiskSpaceStatus ds_status = new DiskSpaceStatus(0.0, 100.0);

            Tuple<EStatusType, double> status = ds_status.GetStatus(0, 1000);
            Assert.Equal(EStatusType.LowOnDiskSpace, status.Item1);
            Assert.Equal(0.0, status.Item2, 1);

            status = ds_status.GetStatus(1, 1000);
            Assert.Equal(EStatusType.LowOnDiskSpace, status.Item1);
            Assert.Equal(0.1, status.Item2, 1);

            status = ds_status.GetStatus(999, 1000);
            Assert.Equal(EStatusType.LowOnDiskSpace, status.Item1);
            Assert.Equal(99.9, status.Item2, 1);

            status = ds_status.GetStatus(1000, 1000);
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.Item1);
            Assert.Equal(100.0, status.Item2, 1);
        }

        [Fact]
        public void HandleThresholdTransitionsProperly()
        {
            DiskSpaceStatus ds_status = new DiskSpaceStatus(80.0, 90.0);

            Tuple<EStatusType, double> status = ds_status.GetStatus(799, 1000);
            Assert.Equal(EStatusType.AdequateDiskSpace, status.Item1);
            Assert.Equal(79.9, status.Item2, 1);

            status = ds_status.GetStatus(800, 1000);
            Assert.Equal(EStatusType.LowOnDiskSpace, status.Item1);
            Assert.Equal(80.0, status.Item2, 1);

            status = ds_status.GetStatus(899, 1000);
            Assert.Equal(EStatusType.LowOnDiskSpace, status.Item1);
            Assert.Equal(89.9, status.Item2, 1);

            status = ds_status.GetStatus(900, 1000);
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.Item1);
            Assert.Equal(90.0, status.Item2, 1);
        }

        [Fact]
        public void HandleZeroCapacityProperly()
        {
            DiskSpaceStatus ds_status = new DiskSpaceStatus(80.0, 90.0);

            Tuple<EStatusType, double> status = ds_status.GetStatus(0, 0);
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.Item1);
            Assert.Equal(0.0, status.Item2, 1);
        }
    }
}
