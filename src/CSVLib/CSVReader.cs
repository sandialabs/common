using System;
using System.IO;

namespace CSVLib
{
    /// <summary>
    /// Class used to read a CSV file.Create an object with the CSV filename,
    /// then call doRead(), providing an object to a class that implements the
    /// ICSVReader interface. The oCSVEntry method in that object will be repeatedly
    /// called for each line in the file.
    /// </summary>
    public class CSVReader
     {
        public CSVReader(string file)
        {
            m_file = file;
        }

        public void doRead(ICSVReader csvEntry)
        {
            try
            {
                FileInfo f = new FileInfo(m_file);
                string[] lines = File.ReadAllLines(f.FullName);

                int lineCount = 0;
                foreach (String line in lines)
                {
                    string[] entries = line.Split(',');
                    csvEntry.OnCSVEntry(lineCount++, entries);
                }
            }
            catch (Exception ex)
            {
                csvEntry.OnError(ex.Message);
            }
        }

        private string m_file;
    }
}
