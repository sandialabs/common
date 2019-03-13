using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.interpreters
{
    public class SMARTInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data data, SQLiteConnection conn)
        {
            if (data.Type == ECollectorType.SMART)
            {
                ListData<HardDisk> disks = data as ListData<HardDisk>;
                if (disks == null)
                    return;

                long device_id = GetDeviceID(data, conn);
                if(device_id >= 0)
                {
                    List<string> drive_letter_list = HardDisk.FailingDrives(disks.Data);
                    SmartStatus smart_status = new SmartStatus();
                    EStatusType status = smart_status.GetStatus(drive_letter_list);

                    SetDeviceStatus(device_id, status, SmartStatus.Types, drive_letter_list.JoinStrings(", "), conn);
                }
            }
        }
    }
}
