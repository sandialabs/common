using gov.sandia.sld.common.configuration;
using System;
using System.Collections.Generic;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    /// <summary>
    /// Collect SMART data through WMI. The code was developed upon referencing:
    /// http://derekwilson.net/blog/2017/08/26/smart
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/3ce1e2fb-6ccf-4340-9d95-fa4895073871/c-and-smart-information-about-hard-disk-drive?forum=netfxbcl
    /// https://social.msdn.microsoft.com/Forums/en-US/af01ce5d-b2a6-4442-b229-6bb32033c755/using-wmi-to-get-smart-status-of-a-hard-disk?forum=vbgeneral
    /// </summary>
    public class SMARTCollector : MultiPropertyWMIDataCollector
    {
        public SMARTCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_DiskDrive", "Model,DeviceID,PNPDeviceID,InterfaceType,SerialNumber", remote_info),
                  new DataCollectorContext(id, ECollectorType.SMART))
        {
        }

        public override CollectedData OnAcquire()
        {
            bool success = true;
            List<HardDisk> disks = new List<HardDisk>();
            CollectedData cd = new CollectedData(Context, success);

            OnAcquireDelegate(
                dict =>
                {
                    string serial_num = string.Empty;
                    if (dict.TryGetValue("SerialNumber", out object o))
                        serial_num = o.ToString().Trim();

                    if (string.IsNullOrEmpty(serial_num) == false)
                    {
                        HardDisk d = new HardDisk()
                        {
                            DeviceID = dict["DeviceID"]?.ToString().Trim(),
                            Model = dict["Model"]?.ToString().Trim(),
                            PnpDeviceID = dict["PNPDeviceID"]?.ToString().Trim(),
                            InterfaceType = dict["InterfaceType"]?.ToString().Trim(),
                            SerialNum = serial_num
                        };
                        disks.Add(d);
                    }
                });

            if (disks.Count > 0)
            {
                ListData<HardDisk> disks2 = new ListData<HardDisk>(Context);
                foreach (HardDisk disk in disks)
                {
                    try
                    {
                        // Figure out which drive letters are on this hard disk
                        ManagementScope scope = WmiContext.GetManagementScope();
                        string drive_pnp = disk.PnpDeviceID.Replace("\\", "\\\\");
                        string queryStr = string.Format("ASSOCIATORS OF {{Win32_DiskDrive.DeviceID='{0}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition", disk.DeviceID);
                        //Console.WriteLine(queryStr);
                        foreach (ManagementBaseObject partition in new ManagementObjectSearcher(scope, new ObjectQuery(queryStr)).Get())
                        {
                            queryStr = string.Format("ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{0}'}} WHERE AssocClass = Win32_LogicalDiskToPartition", partition["DeviceID"]);
                            //Console.WriteLine(queryStr);
                            foreach (ManagementBaseObject o in new ManagementObjectSearcher(scope, new ObjectQuery(queryStr)).Get())
                            {
                                string drive_letter = o["Name"].ToString().Trim().ToUpper();
                                if (disk.DriveLetters.Contains(drive_letter) == false)
                                    disk.DriveLetters.Add(drive_letter);
                            }
                        }

                        // See if the drive is predicting failure
                        scope = WmiContext.GetManagementScope("WMI");
                        queryStr = string.Format("SELECT * FROM MSStorageDriver_FailurePredictStatus WHERE InstanceName LIKE \"%{0}%\"", drive_pnp);
                        //Console.WriteLine(queryStr);
                        foreach (ManagementBaseObject m in new ManagementObjectSearcher(scope, new ObjectQuery(queryStr)).Get())
                        {
                            object failure = m["PredictFailure"];
                            //Console.WriteLine("PredictFailure: " + failure.ToString() + "\n");
                            disk.FailureIsPredicted = (bool)failure;

                            //SMARTFailureRequest req = new SMARTFailureRequest("SMARTCollector");
                            //RequestBus.Instance.MakeRequest(req);
                            //if (req.IsHandled)
                            //    disk.FailureIsPredicted = req.FailureIsPredicted;
                        }

                        // Now get the SMART attributes
                        queryStr = string.Format("SELECT * FROM MSStorageDriver_FailurePredictData WHERE InstanceName LIKE \"%{0}%\"", drive_pnp);
                        //Console.WriteLine(queryStr);
                        foreach (ManagementBaseObject m in new ManagementObjectSearcher(scope, new ObjectQuery(queryStr)).Get())
                        {
                            Byte[] attributes = (Byte[])m.Properties["VendorSpecific"].Value;

                            //Console.WriteLine("Attributes length [A]: {0}", attributes.Length);

                            int num_attributes = attributes.Length / (int)ESmartField.NumSmartFields;
                            for (int i = 0; i < num_attributes; ++i)
                            {
                                try
                                {
                                    byte[] field = new byte[(int)ESmartField.NumSmartFields];
                                    Array.Copy(attributes, i * (int)ESmartField.NumSmartFields, field, 0, (int)ESmartField.NumSmartFields);

                                    ESmartAttribute attr = (ESmartAttribute)field[(int)ESmartField.Attribute];
                                    if (attr == ESmartAttribute.Invalid)
                                        continue;

                                    //int flags = bytes[i * 12 + 4]; // least significant status byte, +3 most significant byte, but not used so ignored.
                                    //                               //bool advisory = (flags & 0x1) == 0x0;
                                    //bool failureImminent = (flags & 0x1) == 0x1;
                                    //bool onlineDataCollection = (flags & 0x2) == 0x2;

                                    int value = field[(int)ESmartField.Value];
                                    //int worst = field[(int)ESmartField.Worst];
                                    //int vendordata = BitConverter.ToInt32(field, (int)ESmartField.VendorData1);

                                    SmartAttribute resource = new SmartAttribute(attr)
                                    {
                                        Value = value
                                    };
                                    disk.SmartAttributes.Add(resource);
                                }
                                catch (Exception ex)
                                {
                                    // given key does not exist in attribute collection (attribute not in the dictionary of attributes)
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }

                        disks2.Data.Add(disk);
                    }
                    catch (Exception ex)
                    {
                        cd.SetMessage(ex);
                        success = false;
                    }
                }

                cd.D.Add(disks2);
            }
            else
                success = false;

            cd.DataIsCollected = success;
            return cd;
        }
    }
}
