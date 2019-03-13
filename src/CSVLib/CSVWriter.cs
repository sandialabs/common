using System;
using System.IO;
using System.Text;

namespace CSVLib
{
    /// <summary>
    /// Used to write a series of CSV-style lines to the specified file.
    /// Each entry in the list of lines to write should already be
    /// comma-separated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CSVWriter
    {
        public CSVWriter(string file)
        {
            m_file = file;
        }

        public void doWrite<T>(CSV<T> csv)
        {
            try
            {
                FileInfo file = new FileInfo(m_file);
                File.WriteAllLines(file.FullName, csv.AllLines(), Encoding.UTF8);
            }
            catch (Exception)
            {
            }
        }

        private string m_file;
    }
}
