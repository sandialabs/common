using gov.sandia.sld.common.data.wmi;
using System;
using Xunit;

namespace UnitTest.diskusage
{
    public class DiskUsageFixture
    {
        public string Value { get
            {
                return
 "{\"Value\":{\"C:\":{\"Capacity\":\"128324718592\",\"Free\":\"11121676288\",\"Used\":\"117203042304\"},\"E:\":{\"Capacity\":\"858857140224\",\"Free\":\"100818341888\",\"Used\":\"758038798336\"}}}";
            }
        }
        public UInt64 CCapacity { get { return 128324718592; } }
        public UInt64 CFree { get { return 11121676288; } }
        public UInt64 CUsed { get { return 117203042304; } }
        public UInt64 ECapacity { get { return 858857140224; } }
        public UInt64 EFree { get { return 100818341888; } }
        public UInt64 EUsed { get { return 758038798336; } }

        public DiskUsageFixture()
        {
        }

        public void TestC(DiskUsage c)
        {
            Assert.Equal(CCapacity.ToString(), c.Capacity);
            Assert.Equal(CFree.ToString(), c.Free);
            Assert.Equal(CUsed.ToString(), c.Used);
            Assert.Equal(CCapacity, c.CapacityNum);
            Assert.Equal(CFree, c.FreeNum);
            Assert.Equal(CUsed, c.UsedNum);
            
            // Make sure the math didn't get messed up somehow
            Assert.Equal(CCapacity - CFree, c.UsedNum);
        }

        public void TestE(DiskUsage e)
        {
            Assert.Equal(ECapacity.ToString(), e.Capacity);
            Assert.Equal(EFree.ToString(), e.Free);
            Assert.Equal(EUsed.ToString(), e.Used);
            Assert.Equal(ECapacity, e.CapacityNum);
            Assert.Equal(EFree, e.FreeNum);
            Assert.Equal(EUsed, e.UsedNum);

            // Make sure the math didn't get messed up somehow
            Assert.Equal(ECapacity - EFree, e.UsedNum);
        }
    }
}
