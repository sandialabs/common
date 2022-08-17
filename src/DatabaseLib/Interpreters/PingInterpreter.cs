using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.db.changers;
using gov.sandia.sld.common.logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.interpreters
{
    /// <summary>
    /// Updates the NetworkStatus table to record ping information about what was pinged
    /// </summary>
    public class PingInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type != ECollectorType.Ping)
                return;
            
            if(d is ListData<PingResult>)
            {
                ListData<PingResult> data = d as ListData<PingResult>;

                Dictionary<string, string> ip_to_name_map = new Dictionary<string, string>();
                string sql = "SELECT IPAddress, Name FROM Devices WHERE DateDisabled IS NULL;";
                using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) == false)
                            ip_to_name_map[reader.GetString(0)] = reader.GetString(1);
                    }
                }

                foreach(PingResult result in data.Data)
                {
                    try
                    {
                        string ip = result.Address.ToString();
                        string name = ip;
                        if (ip_to_name_map.ContainsKey(ip))
                            name = ip_to_name_map[ip];
                        sql = $"SELECT IPAddress FROM NetworkStatus WHERE IPAddress = '{ip}';";
                        Changer changer = null;
                        bool existing = false;
                        using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // It already exists--update the status and name in case it's been changed
                                existing = true;
                                changer = new Updater("NetworkStatus", $"IPAddress = '{ip}'", conn);
                            }
                            else
                            {
                                changer = new Inserter("NetworkStatus", conn);
                                changer.Set("IPAddress", ip, false);
                            }

                            if (changer != null)
                            {
                                changer.Set("Name", name, false);
                                changer.Set("SuccessfulPing", result.IsPingable ? 1 : 0);
                                changer.Set("DatePingAttempted", data.CollectedAt);
                                if (result.IsPingable)
                                    changer.Set("DateSuccessfulPingOccurred", data.CollectedAt);
                                else if(!existing)
                                    // It's new, and wasn't pingable, so we need to report that. We'll do that by
                                    // having an empty date/time
                                    changer.Set("DateSuccessfulPingOccurred", "", false, false);
                                // else it exists, but isn't pingable, so leave the DateSuccessfulPingOccurred alone
                            }
                        }

                        if (changer != null)
                            changer.Execute();
                    }
                    catch (Exception e)
                    {
                        ApplicationEventLog log = new ApplicationEventLog();
                        log.LogError($"PingInterpreter -- {result.Address.ToString()}");
                        log.Log(e);
                    }
                }
            }
            else
                throw new Exception("PingInterpreter: data type is wrong");
        }
    }
}
