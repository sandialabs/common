using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.responders
{
    public class EventLogResponder : Responder
    {
        public Database DB { get; set; }

        public override void HandleRequest(Request request)
        {
            if (request is SystemErrorsInfoRequest == false && request is SystemErrorsUpdateRequest == false)
                return;

            Database db = DB != null ? DB : new Database();
            using (SQLiteConnection conn = db.Connection)
            {
                conn.Open();

                if (request is SystemErrorsInfoRequest)
                {
                    HandleInfoRequest(request as SystemErrorsInfoRequest, conn);
                }
                else if (request is SystemErrorsUpdateRequest)
                {
                    SystemErrorsUpdateRequest sys_request = request as SystemErrorsUpdateRequest;

                    Attribute attr = new Attribute();
                    string machine_name = sys_request.MachineName.Replace(' ', '_').ToLower();
                    string section = sys_request.Type.ToString().ToLower();
                    string max_eventlog_path = $"{machine_name}.{section}.max_event_log";

                    attr.Set(max_eventlog_path,
                        JsonConvert.SerializeObject(sys_request.LogData,
                            Formatting.None,
                            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                        conn);
                    sys_request.Handled();
                }
            }
        }

        private static void HandleInfoRequest(SystemErrorsInfoRequest sys_request, SQLiteConnection conn)
        {
            Attribute attr = new Attribute();
            string machine_name = sys_request.MachineName.Replace(' ', '_').ToLower();
            string section = sys_request.Type.ToString().ToLower();
            string max_record_number_path = $"{machine_name}.{section}.max_record_number";
            string max_eventlog_path = $"{machine_name}.{section}.max_event_log";

            // First, let's look for the "old" systems which just had the record number. If we find
            // those we'll convert them to the "new" system which has a JSON object.
            string max = attr.Get(max_record_number_path, conn);

            if (string.IsNullOrEmpty(max) == false &&
                ulong.TryParse(max, out ulong u))
            {
                // Yes, it's using the old system
                sys_request.LogData.MaxRecordNumber = u;
                sys_request.Handled();

                // Change the attribute so it's using the new one
                attr.Set(max_eventlog_path,
                    JsonConvert.SerializeObject(sys_request.LogData,
                        Formatting.None,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    conn);
                attr.Clear(max_record_number_path, conn);
            }
            else
            {
                // Nope...try the new system
                max = attr.Get(max_eventlog_path, conn);
                if (string.IsNullOrEmpty(max) == false)
                {
                    try
                    {
                        EventLogData system_id = JsonConvert.DeserializeObject<EventLogData>(max);
                        if (system_id != null)
                        {
                            sys_request.LogData.Assign(system_id);
                            sys_request.Handled();
                        }
                    }
                    catch (JsonException)
                    {
                    }
                }
            }
        }
    }
}
