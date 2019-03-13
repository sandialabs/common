using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.dailyreport;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace gov.sandia.sld.common.dailyfiles
{
    /// <summary>
    /// Does the writing of the daily files. Can optionally compress them after writing.
    /// </summary>
    public class Writer
    {
        public bool IsTimeToWrite { get { return m_timer.IsTime(); } }

        public Writer()
        {
            // Get a DateTime for 12:01 AM this morning, then add 1 day to get
            // to tomorrow morning. When we hit that we'll generate the daily file
            // for today (and all earlier dates, if need be).
            DateTime now = DateTime.Now;
            DateTime current_day = new DateTime(now.Year, now.Month, now.Day, 0, 1, 0);
            TimeSpan one_day = TimeSpan.FromDays(1);
            m_timer = new IntervalTimer(current_day + one_day, one_day);
        }

        public void DoWrite(SQLiteConnection conn)
        {
            if (GlobalIsRunning.IsRunning == false)
                return;
            if (IsTimeToWrite == false)
                return;

            m_timer.Reset();

            logging.EventLog log = new ApplicationEventLog();

            Stopwatch watch = Stopwatch.StartNew();

            List<DailyReport> reports = new Retriever().GetIncompleteDailyReports(true, conn);
            string directory = Filename.Directory;

            log.LogInformation($"Writing {reports.Count} daily file(s)");

            JsonSerializer serializer = new JsonSerializer();
            foreach (DailyReport report in reports)
            {
                Stopwatch watch2 = Stopwatch.StartNew();

                Filename f = new Filename(report.countryCode, report.siteName, report.day);
                string filename_base = directory + "\\" + f.Name;
                string filename_json = filename_base + ".json";

                using (StreamWriter sw = new StreamWriter(filename_json))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, report);
                }

                CompressDailyFile compress = new CompressDailyFile();
                bool? do_compress = compress.GetValueAsBoolean(conn);
                if(do_compress.HasValue && do_compress.Value)
                {
                    try
                    {
                        using (ZipFile zip = new ZipFile())
                        {
                            ZipEntry entry = zip.AddFile(filename_json);
                            entry.FileName = f.Name + ".json";

                            // Note that we save the filename as zip.json, and not the more
                            // likely json.zip.
                            //
                            // This is because the daily-file-upload-tool just uploads files from
                            // COMMON or whoever else might be generating daily files, and the back-end
                            // processing of those uploaded files uses the extension to do additional processing.
                            // We don't want special processing for compressed JSON files and uncompressed JSON files,
                            // so we use an unconventional naming convention.
                            zip.Save(filename_base + ".zip.json");
                        }

                        // Optionally delete the original JSON file once it's compressed
                        DeleteDailyFileAfterCompression del = new DeleteDailyFileAfterCompression();
                        bool? do_delete = del.GetValueAsBoolean(conn);
                        if(do_delete.HasValue && do_delete.Value)
                            File.Delete(filename_json);
                    }
                    catch (Exception e)
                    {
                        log.LogError(e.Message);
                    }
                }

                log.LogInformation($"Writing {filename_json} took {watch2.ElapsedMilliseconds} ms");
            }

            watch.Stop();
            log.LogInformation($"Done writing {reports.Count} daily file(s). It took {watch.ElapsedMilliseconds} ms");
        }

        private IntervalTimer m_timer;
    }
}
