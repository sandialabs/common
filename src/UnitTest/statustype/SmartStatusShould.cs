using gov.sandia.sld.common.configuration;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.statustype
{
    public class SmartStatusShould
    {
        [Fact]
        public void HandleFailingDrivesProperly()
        {
            List<string> failing_drive_letters = new List<string>();
            SmartStatus smart_status = new SmartStatus();

            EStatusType status = smart_status.GetStatus(failing_drive_letters);
            Assert.Equal(EStatusType.DiskFailureNotPredicted, status);

            failing_drive_letters.Add("C:");
            status = smart_status.GetStatus(failing_drive_letters);
            Assert.Equal(EStatusType.DiskFailurePredicted, status);
        }
    }
}
