using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

/*
    http://www.i-programmer.info/projects/38-windows/208-disk-drive-dangers.html?start=2

    https://web.archive.org/web/20040525225247/http://www.microsoft.com:80/whdc/archive/smartdrv.mspx
*/

namespace gov.sandia.sld.common.configuration
{
    // Each field of SMART data is 12 bytes. Which byte is which isn't particularly well documented, but
    // referring to the i-programmer article above, they look something like the fields specified
    // in ESmartField:
    public enum ESmartField : int
    {
        Unknown0,       // 0
        Unknown1,       // 1
        Attribute,      // 2
        Status,         // 3
        Unknown3,       // 4
        Value,          // 5
        Worst,          // 6
        VendorData1,    // 7
        VendorData2,    // 8
        VendorData3,    // 9
        VendorData4,    // 10
        Unknown4,       // 11

        NumSmartFields, // 12
    }

    public class SmartAttribute
    {
        public SmartAttribute(ESmartAttribute attr)
        {
            this.Attribute = attr;
            m_value = null;
        }

        public ESmartAttribute Attribute { get; set; }
        public string Name { get { return Attribute.GetDescription(); } }

        public int Value { get { return m_value ?? 0; } set { m_value = value; } }

        [JsonIgnore]
        public bool HasValue { get { return m_value.HasValue; } }

        public override string ToString()
        {
            string str = string.Format("Register: {0} (hex {3:X2}, decimal {3})\nName: {1}\nValue: {2}\n",
                Attribute, Name, Value, (int)Attribute);
            return str;
        }

        private int? m_value;
    }

    public class HardDisk
    {
        public HardDisk()
        {
            DriveLetters = new List<string>();
            SmartAttributes = new List<SmartAttribute>();
            Timestamp = DateTimeOffset.Now;
        }

        public string DeviceID { get; set; }
        public string PnpDeviceID { get; set; }
        public List<string> DriveLetters { get; set; }
        public bool? FailureIsPredicted { get; set; }
        public string Model { get; set; }
        public string InterfaceType { get; set; }
        public string SerialNum { get; set; }
        public List<SmartAttribute> SmartAttributes { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        [JsonIgnore]
        public string DriveLettersAsString { get { DriveLetters.Sort(); return DriveLetters.JoinStrings(", "); } }

        public override string ToString()
        {
            string str = string.Format("Drive:\nDeviceID: {0}\nPnPDeviceID: {1}\nDriveLetters: {2}\nFailureIsPredicted: {3}\nModel: {4}\nType: {5}\nSerial: {6}\nSmartAttributes: {7}",
                DeviceID, PnpDeviceID, DriveLettersAsString, FailureIsPredicted, Model, InterfaceType, SerialNum, SmartAttributes.ToString());
            SmartAttributes.ForEach(a => str += "\n" + a.ToString());
            return str;
        }

        public static List<string> FailingDrives(List<HardDisk> disks)
        {
            List<string> drive_letter_list = new List<string>();
            List<HardDisk> failing = disks.FindAll(d => d.FailureIsPredicted.HasValue ? d.FailureIsPredicted.Value : false);
            failing.ForEach(d => drive_letter_list = drive_letter_list.Union(d.DriveLetters).ToList());
            drive_letter_list.Sort();
            return drive_letter_list;
        }
    }
}
