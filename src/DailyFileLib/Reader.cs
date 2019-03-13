using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.db.dailyreport;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace gov.sandia.sld.common.dailyfiles
{
    /// <summary>
    /// Reads a directory of daily files, and you can get a DailyReport object
    /// from them.
    /// </summary>
    public class Reader
    {
        public string BaseDirectory { get; private set; }

        public Reader(string base_directory)
        {
            BaseDirectory = base_directory;
        }

        public class DailyFileInfo
        {
            public FileInfo Info { get; set; }
            public FileDetails Details { get; set; }
            public DateTimeOffset Day
            {
                get
                {
                    if (_day == DateTimeOffset.MinValue)
                    {
                        JObject o = JObject.Parse(File.ReadAllText(Info.FullName));
                        if (o != null)
                        {
                            JToken t = o["day"];
                            if(t != null)
                                _day = t.ToObject<DateTimeOffset>();
                        }
                    }
                    return _day;
                }
            }

            /// <summary>
            /// Lazy-load the actual report. This will keep us from loading all of them at once.
            /// </summary>
            public DailyReport Report
            {
                get
                {
                    DailyReport report = JsonConvert.DeserializeObject<DailyReport>(File.ReadAllText(Info.FullName));
                    return report;
                }
            }

            private DateTimeOffset _day;
        }

        public List<DailyFileInfo> DoRead()
        {
            List<DailyFileInfo> reports = new List<DailyFileInfo>();
            DirectoryTraverser traverser = new DirectoryTraverser(BaseDirectory, "*.json");
            List<FileInfo> files = traverser.files;

            foreach(FileInfo file in files)
            {
                try
                {
                    FileInformationRequest req = new FileInformationRequest(EFileType.COMMONDailyFile, file.FullName);
                    RequestBus.Instance.MakeRequest(req);
                    if (req.IsHandled)
                    {
                        // Return a report if the ID isn't in the database, or if it has changed since it's been put in the database
                        bool return_report =
                            req.Details == null ||
                            req.ID < 0 ||
                            req.Details.Size != (ulong)file.Length ||
                            req.Details.Modification != file.LastWriteTime;

                        if (return_report)
                        {
                            DailyFileInfo file_info = new DailyFileInfo()
                            {
                                Info = file,
                                Details = req.Details,
                            };
                            reports.Add(file_info);
                        }
                    }
                }
                catch (Exception e)
                {
                    ILog log = LogManager.GetLogger(typeof(Reader));
                    log.Error(e);
                }
            }

            return reports;
        }
    }
}
