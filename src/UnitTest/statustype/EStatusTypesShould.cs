using gov.sandia.sld.common.configuration;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.statustype
{
    public class EStatusTypesShould
    {
        [Fact]
        public void JoinOK()
        {
            List<EStatusType> types = new List<EStatusType>(new EStatusType[] { EStatusType.ExcessiveCPU, EStatusType.GoodPing, EStatusType.Online });
            string join = types.Join();
            Assert.Equal("10,12,1", join);
            types.Add(EStatusType.DiskFailurePredicted);
            join = types.Join();
            Assert.Equal("10,12,1,16", join);
        }

        [Fact]
        public void DiskSpaceCompareIsOK()
        {
            EStatusType status = EStatusType.AdequateDiskSpace;
            Assert.Equal(EStatusType.LowOnDiskSpace, status.DiskSpaceCompare(EStatusType.LowOnDiskSpace));
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.DiskSpaceCompare(EStatusType.CriticallyLowOnDiskSpace));

            status = EStatusType.LowOnDiskSpace;
            Assert.Equal(EStatusType.LowOnDiskSpace, status.DiskSpaceCompare(EStatusType.AdequateDiskSpace));
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.DiskSpaceCompare(EStatusType.CriticallyLowOnDiskSpace));

            status = EStatusType.CriticallyLowOnDiskSpace;
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.DiskSpaceCompare(EStatusType.AdequateDiskSpace));
            Assert.Equal(EStatusType.CriticallyLowOnDiskSpace, status.DiskSpaceCompare(EStatusType.LowOnDiskSpace));
        }
    }
}
