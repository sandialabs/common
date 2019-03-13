using gov.sandia.sld.common.logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// Used so a single database file can be used by different applications, such as the COMMON service
    /// and the web server. The file common_config.json holds a DatabaseConfiguration object that points
    /// to where the single database file. Each application will have its own common_config.json file.
    /// 
    /// The default name of the database is common.sqlite
    /// 
    /// </summary>
    public class Context
    {
        public string DBPath { get; set; }

        public Context(FileInfo f = null)
        {
            DBPath = (f == null) ? c_default_path : f.FullName;
        }

        public void Save()
        {
            using (StreamWriter file = File.CreateText(c_filename))
            using (JsonWriter jw = new JsonTextWriter(file))
            {
                jw.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Include;
                serializer.Serialize(jw, this);
            }
        }

        public static void SpecifyFilename(string filename)
        {
            c_specified_filename = filename;
        }

        public static Context LoadConfigFromFile()
        {
            Context config = new Context();

            if (string.IsNullOrEmpty(c_specified_filename) == false)
                config.DBPath = c_specified_filename;
            else
            {
                try
                {
                    FileInfo fi = new FileInfo(c_filename);
                    using (StreamReader file = File.OpenText(fi.FullName))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        config = (Context)serializer.Deserialize(file, typeof(Context));

                        c_specified_filename = config.DBPath;
                    }
                }
                catch (Exception ex)
                {
                    if (File.Exists(c_filename) == false)
                        config.Save();

                    ILog log = LogManager.GetLogger(typeof(Context));
                    log.Error(ex.Message);
                }
            }

            return config;
        }

        private static readonly string c_filename = @".\common_config.json";
        private static readonly string c_default_path = @".//common.sqlite";
        private static string c_specified_filename = string.Empty;
    }
}
