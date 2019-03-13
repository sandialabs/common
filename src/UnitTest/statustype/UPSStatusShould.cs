using gov.sandia.sld.common.configuration;
using Xunit;

namespace UnitTest.statustype
{
    public class UPSStatusShould
    {
        [Fact]
        public void HandleBatteryStatusesProperly()
        {
            EUPSBatteryStatus battery_status = EUPSBatteryStatus.Charging;
            UPSStatus ups_status = new UPSStatus();

            EStatusType status = ups_status.GetStatus(battery_status);
            Assert.Equal(EStatusType.UPSNotProvidingPower, status);

            battery_status = EUPSBatteryStatus.Other;
            status = ups_status.GetStatus(battery_status);
            Assert.Equal(EStatusType.UPSProvidingPower, status);
        }
    }
}
