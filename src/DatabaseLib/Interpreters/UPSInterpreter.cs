using System.Data.SQLite;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.utilities;
using System;

namespace gov.sandia.sld.common.db.interpreters
{
    public class UPSInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type != ECollectorType.UPS)
                return;
            
            if(d is DictionaryData)
            {
                long device_id = GetDeviceID(d, conn);
                if (device_id < 0)
                    return;

                DictionaryData dict = d as DictionaryData;

                int? battery_status = null;
                if (dict.Data.ContainsKey("BatteryStatusInt"))
                {
                    string battery_status_str = dict.Data["BatteryStatusInt"];
                    if (int.TryParse(battery_status_str, out int i))
                        battery_status = i;
                }
                else if(dict.Data.ContainsKey("BatteryStatus"))
                {
                    string status = dict.Data["BatteryStatus"];
                    string other_status = EUPSBatteryStatus.Other.GetDescription();
                    battery_status = (string.Compare(status, other_status, true) == 0) ? (int)EUPSBatteryStatus.Other : (int)EUPSBatteryStatus.Undefined;
                }

                if(battery_status.HasValue)
                {
                    EUPSBatteryStatus status = (EUPSBatteryStatus)battery_status.Value;
                    UPSStatus ups_status = new UPSStatus();

                    SetDeviceStatus(device_id, ups_status.GetStatus(status), UPSStatus.Types, status.GetDescription(), conn);
                }
            }
            else
                throw new Exception("UPSInterpreter: data type is wrong");
        }
    }
}
