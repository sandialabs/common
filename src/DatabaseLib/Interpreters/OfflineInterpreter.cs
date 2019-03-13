using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.interpreters
{
    /// <summary>
    /// Updates the device status to show whether the device is online/offline, and also if it has good or
    /// slow ping response.
    /// </summary>
    public class OfflineInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type != ECollectorType.Ping)
                return;

            if (d is ListData<PingResult>)
            {
                ListData<PingResult> data = d as ListData<PingResult>;

                Dictionary<string, long> ip_to_device_id_map = new Dictionary<string, long>();
                string sql = "SELECT IPAddress, DeviceID FROM Devices WHERE DateDisabled IS NULL;";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) == false)
                            ip_to_device_id_map[reader.GetString(0)] = reader.GetInt64(1);
                    }
                }

                foreach (PingResult result in data.Data)
                {
                    string ip = result.Address.ToString();
                    if (ip_to_device_id_map.TryGetValue(ip, out long device_id))
                    {
                        Tuple<EStatusType, EStatusType?> ping_status = PingStatus.GetStatus(result.IsPingable, result.AvgTime);

                        SetDeviceStatus(device_id, ping_status.Item1, PingStatus.OnlineOrOffline, string.Empty, conn);

                        // If the device isn't pingable, make sure we clear the good-or-slow statuses as well
                        if (result.IsPingable == false)
                            ClearDeviceStatus(device_id, PingStatus.GoodOrSlow, conn);

                        if (ping_status.Item2.HasValue)
                            SetDeviceStatus(device_id, ping_status.Item2.Value, PingStatus.GoodOrSlow, string.Empty, conn);
                    }
                    else
                    {
                        ApplicationEventLog log = new ApplicationEventLog();
                        log.LogError($"OfflineInterpreter: unable to find device_id from {ip}");
                    }
                }
            }
            else
            {
                string json = JsonConvert.SerializeObject(d);
                throw new Exception($"OfflineInterpreter: data type is wrong {json}");
            }
        }
    }
}
