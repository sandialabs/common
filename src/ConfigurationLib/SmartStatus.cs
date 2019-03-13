using System.Collections.Generic;

namespace gov.sandia.sld.common.configuration
{
    public class SmartStatus
    {
        public static List<EStatusType> Types
        {
            get =>
                new List<EStatusType>(new EStatusType[]
                {
                    EStatusType.DiskFailurePredicted,
                    EStatusType.DiskFailureNotPredicted
                });
        }

        public EStatusType GetStatus(List<string> failing_drive_letters)
        {
            return failing_drive_letters.Count > 0 ? EStatusType.DiskFailurePredicted : EStatusType.DiskFailureNotPredicted;
        }
    }
}
