using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.responders
{
    public class MonitoredDrivesResponder : IResponder
    {
        public void HandleRequest(IRequest request)
        {
            if(request is MonitoredDrivesRequest)
            {
                MonitoredDrivesRequest md = request as MonitoredDrivesRequest;

                Database db = new Database();
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    string path = md.MachineName + ".monitored_drives";
                    string value = Configuration.GetValue(path, conn);
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        try
                        {
                            MonitoredDriveManager manager = JsonConvert.DeserializeObject<MonitoredDriveManager>(value);
                            if (manager != null)
                            {
                                md.DriveManager = manager;
                                md.Handled();
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
    }
}
