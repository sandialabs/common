using gov.sandia.sld.common.db;
using System;
using System.Data.SQLite;

namespace gov.sandia.sld.common.dailyfiles
{
    /// <summary>
    /// The format of the daily file filename, and the location where the daily files are written
    /// </summary>
    public class Filename
    {
        public string Name { get; private set; }

        public static string Directory
        {
            get
            {
                string directory = AppDomain.CurrentDomain.BaseDirectory + @"\DailyFiles";
                Database db = new Database();
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();
                    DailyFileLocation loc = new DailyFileLocation(directory);
                    directory = loc.GetValue(conn);
                }

                if (!System.IO.Directory.Exists(directory))
                    System.IO.Directory.CreateDirectory(directory);

                return directory;
            }
        }

        public Filename(string country_code, string site_name, DateTimeOffset day)
        {
            // Replace spaces in the site name with dashes (not underscores) so we can more easily
            // parse the filename should we need to.
            site_name = site_name.ToLower().Replace(' ', '-');
            country_code = country_code.ToLower();

            Name = $"{country_code}_{site_name}_{day.Year:D4}-{day.Month:D2}-{day.Day:D2}";
        }
    }
}
