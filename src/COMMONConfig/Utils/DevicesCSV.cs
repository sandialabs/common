using CSVLib;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace COMMONConfig.Utils
{
    public class DevicesCSV : CSV<DeviceInfo>
    {
        public DevicesCSV(List<DeviceInfo> devices, string filename) : base(filename)
        {
            foreach(DeviceInfo d in devices)
            {
                // Skip the System device and the Server device
                if (d.type != EDeviceType.System && d.type != EDeviceType.Server)
                    Add(d);
            }
        }

        public DevicesCSV(string filename) : base(filename)
        {
        }

        public void Load()
        {
            Entries.Clear();
            Init();
        }

        public void Save()
        {
            Header = DeviceLine.Header;
            WriteFile();
        }

        public override DeviceInfo Generate(int line, string[] entries)
        {
            DeviceInfo di = null;

            try
            {
                DeviceLine dl = new DeviceLine(line, entries);
                di = new DeviceInfo(dl.IsWindows ? EDeviceType.Workstation : EDeviceType.Generic)
                {
                    DID = new DeviceID(-1, dl.Name),
                    ipAddress = dl.IPAddress
                };
            }
            catch (Exception ex)
            {
                // It's not an error if the first line chokes on the "Is Windows" column.
                if (string.Compare(ex.Message, "Invalid IsWindows field: Is Windows (line 0)", true) != 0)
                    throw ex;
            }

            return di;
        }

        public override string GenerateLine(DeviceInfo t)
        {
            DeviceLine dl = new DeviceLine(t);
            return dl.ToLine();
        }

        public class DeviceLine
        {
            public static string Header { get { return "Name,IP Address,Is Windows"; } }

            public string Name { get; set; }
            public string IPAddress { get; set; }
            public bool IsWindows { get; set; }

            public DeviceLine()
            {
                Name = IPAddress = string.Empty;
                IsWindows = false;
            }

            public DeviceLine(int line, string[] entries)
            {
                if (entries.Count() >= 3)
                {
                    Name = entries[0];
                    IPAddress = entries[1];
                    string is_windows = entries[2];
                    if (is_windows == "0" || is_windows == "1")
                        IsWindows = is_windows == "1";
                    else
                        throw new Exception(string.Format("Invalid IsWindows field: {0} (line {1})", is_windows, line));
                }
                else
                    throw new Exception(string.Format("Invalid entries count {0} (line {1})", entries.Count(), line));
            }

            public DeviceLine(DeviceInfo info)
            {
                Name = info.name;
                IPAddress = info.ipAddress;
                IsWindows = info.type == EDeviceType.Server || info.type == EDeviceType.Workstation;
            }

            public string ToLine()
            {
                string line = string.Format("{0},{1},{2}", Name.Replace(",", ""), IPAddress, IsWindows ? "1" : "0");
                return line;
            }
        }
    }
}
