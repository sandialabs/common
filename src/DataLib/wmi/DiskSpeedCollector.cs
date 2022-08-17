using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gov.sandia.sld.common.data.wmi
{
    public class DiskSpeedCollector : MultiPropertyWMIDataCollector
    {
        public DiskSpeedCollector(CollectorID id, Remote remote_info)
            : base(new WMIContext("Win32_PerfFormattedData_PerfDisk_PhysicalDisk", "PercentDiskTime,AvgDiskQueueLength,Name", remote_info),
                  new DataCollectorContext(id, ECollectorType.DiskSpeed))
        {
        }

        public override CollectedData OnAcquire()
        {
            List<string> drive_names = new List<string>();
            AttributeRequest request = new AttributeRequest(Context.ID.Name + ".all.drives.descriptions", true);
            SystemBus.Instance.MakeRequest(request);
            if (request.IsHandled)
            {
                try
                {
                    Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.Value);
                    drive_names = new List<string>(dict.Keys);
                }
                catch (Exception)
                {
                }
            }

            CollectedData basedata = base.OnAcquire();
            List<Data> dataList = new List<Data>();
            if (basedata.DataIsCollected)
            {
                List<DictionaryData> speedData = basedata.D.Select(w => w as DictionaryData).ToList();
                while (speedData.Count > 0)
                {
                    var speed = speedData[0];
                    speedData.RemoveAt(0);

                    if (speed.Data.ContainsKey("PercentDiskTime") == false ||
                        speed.Data.ContainsKey("AvgDiskQueueLength") == false ||
                        speed.Data.ContainsKey("Name") == false)
                    {
                        continue;
                    }

                    var d = new DictionaryData(Context);
                    string drive_name = speed.Data["Name"].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Last();
                    d.Data["Disk Time %"] = speed.Data["PercentDiskTime"];
                    d.Data["Avg Disk Q Length"] = speed.Data["AvgDiskQueueLength"];
                    d.Data["Disk Name"] = drive_name;
                    bool is_total = drive_name.ToLower().Contains("total");
                    if (is_total == false &&
                        (drive_names.Count == 0 || drive_names.Contains(drive_name.ToUpper())))
                    {
                        dataList.Add(d);
                    }
                }
            }

            return new CollectedData(Context, basedata.DataIsCollected && dataList.Count > 0, dataList);
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
