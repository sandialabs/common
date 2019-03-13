using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace gov.sandia.sld.common.db.interpreters
{
    /// <summary>
    /// Generate an alert if the machine has been recently rebooted or has been up for an excessively long time
    /// 
    /// The thresholds for recent and excessively long are paramaterized in the database but default to 0 and 365 days.
    /// </summary>
    public class TimeInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type == ECollectorType.Uptime)
            {
                Match m = _regex.Match(d.Value);

                if(m.Success)
                {
                    RecentRebootAlert recent_alert = new RecentRebootAlert();
                    LongRebootAlert long_alert = new LongRebootAlert();
                    int r_alert = recent_alert.GetValueAsInt(conn) ?? 0;
                    int l_alert = long_alert.GetValueAsInt(conn) ?? 365;
                    TimeStatus time_status = new TimeStatus(r_alert, l_alert);

                    if (ulong.TryParse(m.Groups[1].ToString(), out ulong days))
                    {
                        long device_id = GetDeviceID(d, conn);
                        if (device_id >= 0)
                        {
                            EStatusType? type = time_status.GetStatus(days);

                            SetDeviceStatus(device_id, type, TimeStatus.Types, string.Empty, conn);
                        }
                        else
                            throw new Exception($"Error getting device ID: {d.Name}");
                    }
                    else
                        throw new Exception($"Error parsing days: {m.Groups[1]}");
                }
                else
                    throw new Exception($"Error matching '{d.Value}'");
            }
        }

        private readonly Regex _regex = new Regex(@"(\d+) \d+:\d+:\d+");
    }
}
