using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gov.sandia.sld.common.data.wmi
{
    public class DiskUsage
    {
        public string Capacity { get { return CapacityNum.ToString(); } set { UInt64 c; if (UInt64.TryParse(value, out c)) CapacityNum = c; } }
        public string Free { get { return FreeNum.ToString(); } set { UInt64 f; if (UInt64.TryParse(value, out f)) FreeNum = f; } }
        public string Used { get { return UsedNum.ToString(); } }

        [JsonIgnore]
        public UInt64 CapacityNum { get; set; }
        [JsonIgnore]
        public UInt64 FreeNum { get; set; }
        [JsonIgnore]
        public UInt64 UsedNum { get { return CapacityNum - FreeNum; } }
    }

    public class DiskUsageCollector : MultiPropertyWMIDataCollector
    {
        public DiskUsageCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_LogicalDisk", "Size,FreeSpace,Name,DriveType,VolumeName", remote_info),
                  new DataCollectorContext(id, ECollectorType.Disk))
        {
        }

        public override CollectedData OnAcquire()
        {
            CollectedData cd = base.OnAcquire();
            CollectedData return_data = new CollectedData(Context, false);

            if (cd.DataIsCollected)
            {
                List<DictionaryData> usage_data = cd.D.Select(w => w as DictionaryData).ToList();
                //MonitoredDrivesRequest request = new MonitoredDrivesRequest(Context.Name);
                //RequestBus.Instance.MakeRequest(request);
                List<Data> data = new List<Data>();

                // Package the drive information into a single Data object that maps the drive letter to a DiskUsage
                // object that maintains the capacity/free/used values for that drive.
                GenericDictionaryData<DiskUsage> d = new GenericDictionaryData<DiskUsage>(Context);
                //data.Add(d);

                // Unfortunately, there's no way to determine the drive type when determining the disk speeds (see DiskSpeed class).
                // So that we're reporting the same drives, record the drive letters here, then match them when we do the disk speed.
                // And we also want to keep track of the drive letter => name ("C:" => "OSDisk", for example), so let's grab that now
                // as well.
                Dictionary<string, string> drive_descriptions = new Dictionary<string, string>();

                while (usage_data.Count > 0)
                {
                    DictionaryData usage = usage_data[0];
                    usage_data.RemoveAt(0);

                    // Only report local disks. Don't do removeable (floppy or thumb drive), CDs, or network drives.
                    if (int.TryParse(usage.Data["DriveType"], out int drive_type) &&
                        drive_type == (int)EDriveType.LocalDisk &&
                        usage.Data.ContainsKey("Size") &&
                        usage.Data.ContainsKey("FreeSpace") &&
                        usage.Data.ContainsKey("Name") &&
                        usage.Data.ContainsKey("VolumeName"))
                    {
                        string drive_name = usage.Data["Name"].Trim();
                        string description = usage.Data["VolumeName"].Trim();
                        drive_descriptions[drive_name] = description;

                        if (ulong.TryParse(usage.Data["Size"], out ulong c) &&
                            ulong.TryParse(usage.Data["FreeSpace"], out ulong f))
                        {
                            d.Data[drive_name] = new DiskUsage() { CapacityNum = c, FreeNum = f };
                        }
                    }
                }

                if (d.Data.Count > 0)
                {
                    return_data.DataIsCollected = true;
                    return_data.D.Add(d);
                }

                // Record the drive descriptions as an attribute in the database. We will also use this to get the drive letters
                // when needed.
                string json = JsonConvert.SerializeObject(drive_descriptions);
                AttributeRequest attr_request = new AttributeRequest(Context.ID.Name + ".all.drives.descriptions", false) { Value = json };
                SystemBus.Instance.MakeRequest(attr_request);
            }

            return return_data;
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            Data d = null;
            var definition = new { Value = new Dictionary<string, DiskUsage>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d = new GenericDictionaryData<DiskUsage>(context) { Data = data.Value };
            return d;
        }
    }
}
