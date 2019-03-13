using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using System.Linq;

namespace gov.sandia.sld.common.configuration
{
    public class CPUStatus
    {
        public static List<EStatusType> Types
        {
            get =>
                new List<EStatusType>(new EStatusType[] {
                    EStatusType.ExcessiveCPU,
                    EStatusType.NormalCPU
                });
        }

        public PercentThreshold T { get; private set; }
        public double Average { get; private set; }

        public CPUStatus(double low)
        {
            T = new PercentThreshold(low);
            Average = -1.0;
        }

        public EStatusType GetStatus(List<int> cpu)
        {
            bool alert = false;
            if (cpu.Count > 0)
            {
                // Average() throws an exception if the list is empty
                Average = cpu.Average();
                alert = Average >= T.Transition;
            }

            return alert ? EStatusType.ExcessiveCPU : EStatusType.NormalCPU;
        }
    }
}
