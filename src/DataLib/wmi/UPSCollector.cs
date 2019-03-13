using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.wmi
{
    public class UPSCollector : WMIDataCollector
    {
        public UPSCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_Battery", "BatteryStatus,EstimatedChargeRemaining,EstimatedRunTime,Name,Status", remote_info),
                  new DataCollectorContext(id, ECollectorType.UPS))
        {
        }

        public override CollectedData OnAcquire()
        {
            DictionaryData d = null;

            OnAcquireDelegate(
                dict =>
                {
                    d = new DictionaryData(Context);

                    string battery_status_str = dict["BatteryStatus"].ToString();
                    int battery_status = int.Parse(battery_status_str);
                    EUPSBatteryStatus eups_battery_status = (EUPSBatteryStatus)battery_status;

                    d.Data["BatteryStatusInt"] = battery_status_str;
                    d.Data["BatteryStatus"] = eups_battery_status.GetDescription();
                    d.Data["EstimatedChargeRemaining"] = dict["EstimatedChargeRemaining"].ToString();

                    // https://superuser.com/questions/618589/win32-battery-estimatedruntime
                    string estimated_run_time_str = dict["EstimatedRunTime"].ToString();
                    if(estimated_run_time_str == "71582788")
                        estimated_run_time_str = "Charging";
                    d.Data["EstimatedRunTime"] = estimated_run_time_str;

                    d.Data["Name"] = dict["Name"].ToString();
                    d.Data["Status"] = dict["Status"].ToString();
                });

            return new CollectedData(Context, d != null, d);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            DictionaryData d = new DictionaryData(context);
            var definition = new { Value = new Dictionary<string, string>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d.Data = data.Value;
            return d;
        }
    }
}
