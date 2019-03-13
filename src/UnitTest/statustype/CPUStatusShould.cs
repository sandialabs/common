using gov.sandia.sld.common.configuration;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.statustype
{
    public class CPUStatusShould
    {
        [Fact]
        public void GenerateStatusTypesProperly()
        {
            CPUStatus cpu_status = new CPUStatus(90.0);
            List<int> cpu = new List<int>(new int[] { 1, 1, 1, 1, 1 });

            EStatusType status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.NormalCPU, status);
            Assert.Equal(1.0, cpu_status.Average, 1);

            cpu = new List<int>(new int[] { 90, 90, 90, 90, 90 });
            status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.ExcessiveCPU, status);
        }

        [Fact]
        public void HandleMaxThresholdsProperly()
        {
            CPUStatus cpu_status = new CPUStatus(0.0);
            List<int> cpu = new List<int>(new int[] { 0, 0, 0, 0, 0 });

            EStatusType status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.ExcessiveCPU, status);

            cpu_status = new CPUStatus(100.0);
            status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.NormalCPU, status);
            cpu = new List<int>(new int[] { 100, 100, 100, 100, 100 });
            status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.ExcessiveCPU, status);
        }

        [Fact]
        public void HandleThresholdTransitionsProperly()
        {
            CPUStatus cpu_status = new CPUStatus(90.0);
            List<int> cpu = new List<int>(new int[] { 89, 89, 89, 89, 89 });

            EStatusType status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.NormalCPU, status);
            Assert.Equal(89.0, cpu_status.Average, 1);

            cpu = new List<int>(new int[] { 89, 90, 90, 90, 90 });
            status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.NormalCPU, status);
            Assert.Equal(89.8, cpu_status.Average, 1);

            cpu[0] = 90;
            status = cpu_status.GetStatus(cpu);
            Assert.Equal(EStatusType.ExcessiveCPU, status);
            Assert.Equal(90.0, cpu_status.Average, 1);
        }
    }
}
