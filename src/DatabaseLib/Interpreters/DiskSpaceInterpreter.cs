using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.wmi;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.interpreters
{
    public class DiskSpaceInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type != ECollectorType.Disk)
                return;
            
            if(d is GenericDictionaryData<DiskUsage>)
            {
                Dictionary<string, DiskUsage> value = ((GenericDictionaryData<DiskUsage>)d).Data;

                // Don't alert on drives that aren't being monitored
                MonitoredDrivesRequest request = new MonitoredDrivesRequest(d.Name);
                SystemBus.Instance.MakeRequest(request);

                DiskSpaceLowAlert low_level = new DiskSpaceLowAlert();
                DiskSpaceCriticallyLowAlert critically_low_level = new DiskSpaceCriticallyLowAlert();
                double low = low_level.GetValueAsDouble(conn) ?? 80.0;
                double critically_low = critically_low_level.GetValueAsDouble(conn) ?? 90.0;

                EStatusType status = EStatusType.AdequateDiskSpace;
                List<string> messages = new List<string>();
                DiskSpaceStatus ds_status = new DiskSpaceStatus(low, critically_low);

                foreach (string drive in value.Keys)
                {
                    if (request.IsHandled && request.DriveManager.IsDriveMonitored(drive) == false)
                        continue;

                    DiskUsage data = value[drive];
                    Tuple<EStatusType, double> drive_status = ds_status.GetStatus(data.UsedNum, data.CapacityNum);
                    status = status.DiskSpaceCompare(drive_status.Item1);

                    if (drive_status.Item1 == EStatusType.CriticallyLowOnDiskSpace || drive_status.Item1 == EStatusType.LowOnDiskSpace)
                        messages.Add($"{drive} -- {drive_status.Item2:0.0} %");
                }

                string message = messages.JoinStrings(", ");
                long device_id = GetDeviceID(d, conn);
                if (device_id >= 0)
                    SetDeviceStatus(device_id, status, DiskSpaceStatus.Types, message, conn);
                else
                {
                    ApplicationEventLog log = new ApplicationEventLog();
                    log.LogError($"DiskSpaceInterpreter: unable to get device id for {d.Context.Name}");
                }
            }
            else
                throw new Exception("DiskSpaceInterpreter: data type is wrong");
        }
    }
}
