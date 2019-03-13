using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gov.sandia.sld.common.data.wmi
{
    public class DiskNameCollector : MultiPropertyWMIDataCollector
    {
        public DiskNameCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_LogicalDisk", "Name,VolumeName,DriveType", remote_info),
                  new DataCollectorContext(id, ECollectorType.Disk))
        {
        }

        public override CollectedData OnAcquire()
        {
            CollectedData cd = base.OnAcquire();
            CollectedData return_data = new CollectedData(Context, false);

            if (cd.DataIsCollected)
            {
                List<DictionaryData> drive_data = cd.D.Select(w => w as DictionaryData).ToList();
                List<Data> data = new List<Data>();

                // Package the drive information into a single Data object that maps the drive letter to a DiskName
                // object that maintains the drive's values
                GenericDictionaryData<DriveInfo> d = new GenericDictionaryData<DriveInfo>(Context);
                while (drive_data.Count > 0)
                {
                    DictionaryData usage = drive_data[0];
                    drive_data.RemoveAt(0);

                    // Only report local disks. Don't do removeable (floppy or thumb drive), CDs, or network drives.
                    int drive_type;
                    if (int.TryParse(usage.Data["DriveType"], out drive_type) &&
                        drive_type == (int)EDriveType.LocalDisk &&
                        usage.Data.ContainsKey("Name") &&
                        usage.Data.ContainsKey("VolumeName"))
                    {
                        string drive_name = usage.Data["Name"].Trim();
                        string description = usage.Data["VolumeName"].Trim();

                        DriveInfo disk = new DriveInfo()
                        {
                            name = description,
                            letter = drive_name,
                            type = (EDriveType)drive_type
                        };
                        d.Data[drive_name] = disk;
                    }
                }

                if (d.Data.Count > 0)
                {
                    return_data.DataIsCollected = true;
                    return_data.D.Add(d);
                }
            }

            return return_data;
        }
    }
}
