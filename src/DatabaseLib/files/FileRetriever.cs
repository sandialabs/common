using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using System;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.files
{
    public class FileRetriever
    {
        public EFileType Type { get; private set; }
        public string Filename { get; private set; }

        public FileRetriever(EFileType type, string filename)
        {
            Type = type;
            Filename = filename;
        }

        public FileRecord Get()
        {
            FileRecord frecord = null;
            string sql = $"SELECT FileID, FileType, Name, Size, ModificationTime, SHA256 FROM Files WHERE FileType = {(int)Type} AND Name = '{Filename}';";

            Database db = new Database();
            try
            {
                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    using (SQLiteCommand command = new SQLiteCommand(sql, conn))
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            frecord = new FileRecord()
                            {
                                ID = reader.GetInt64(0),
                                Type = (EFileType)reader.GetInt32(1),
                                Details = new FileDetails()
                                {
                                    Name = reader.GetString(2),
                                    Size = (UInt64)reader.GetInt64(3),
                                    Modification = DateTimeOffset.Parse(reader.GetString(4)),
                                    SHA256 = reader.GetString(5),
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger(typeof(FileRetriever));
                log.Error("Get: " + sql);
                log.Error(ex);
            }

            return frecord;
        }
    }
}
