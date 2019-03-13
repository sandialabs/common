using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using Xunit;

namespace UnitTest.wmi
{
    public class HardDiskShould
    {
        [Fact]
        void ConstructOK()
        {
            HardDisk hd = new HardDisk();

            Assert.Null(hd.DeviceID);
            Assert.Null(hd.PnpDeviceID);
            Assert.Empty(hd.DriveLetters);
            Assert.Null(hd.FailureIsPredicted);
            Assert.Null(hd.Model);
            Assert.Null(hd.InterfaceType);
            Assert.Null(hd.SerialNum);
            Assert.Empty(hd.SmartAttributes);
            Assert.Equal(string.Empty, hd.DriveLettersAsString);
        }

        [Fact]
        void ProperlyShowFailedDrives()
        {
            HardDisk hd = new HardDisk()
            {
                DeviceID = "ABC123",
                FailureIsPredicted = true
            };
            hd.DriveLetters.Add("E:");
            hd.DriveLetters.Add("C:");

            Assert.Equal("ABC123", hd.DeviceID);
            Assert.True(hd.FailureIsPredicted.Value);
            Assert.Equal(2, hd.DriveLetters.Count);
            Assert.Equal("C:, E:", hd.DriveLettersAsString);

            HardDisk hd2 = new HardDisk()
            {
                DeviceID = "XYZ789",
                FailureIsPredicted = false
            };
            hd2.DriveLetters.Add("F:");
            hd2.DriveLetters.Add("H:");

            HardDisk hd3 = new HardDisk()
            {
                DeviceID = "EEE777",
                FailureIsPredicted = true
            };
            hd3.DriveLetters.Add("J:");
            hd3.DriveLetters.Add("I:");

            List<HardDisk> drives = new List<HardDisk>(new HardDisk[] { hd, hd2, hd3 });
            List<string> failing = HardDisk.FailingDrives(drives);
            string joined = failing.JoinStrings(", ");

            Assert.Equal("C:, E:, I:, J:", joined);
        }
    }
}
