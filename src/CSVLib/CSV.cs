using System;
using System.Collections.Generic;

namespace CSVLib
{
    public abstract class CSV<T> : ICSVReader, ICSVWriter
    {
        public string Filename { get; set; }
        public List<T> Entries { get; set; }
        public string Header { get; set; }
        public List<string> Errors { get; set; }

	    public CSV(String filename)
        {
            Filename = filename;
            Entries = new List<T>();
            Header = null;
            Errors = new List<string>();
        }

        /// <summary>
        /// Called as a T is created inside the generate method
        /// </summary>
        /// <param name="t">The object to insert</param>
        protected void Add(T t)
        {
            if (t != null)
                Entries.Add(t);
        }

        /// <summary>
        /// Get all of the entries in the CSV as rows of text. Is typically used as the
	    /// data is being serialized back out to file.
        /// </summary>
        /// <returns>The list of strings as each T object is serialized to a CSV line</returns>
        public List<string> AllLines()
        {
            Init();
            List<string> lines = new List<string>();
            if (string.IsNullOrEmpty(Header) == false)
                lines.Add(Header);
            Entries.ForEach(e => lines.Add(GenerateLine(e)));
            return lines;
        }

        /// <summary>
        /// Generate a single T given the entries
        /// </summary>
        /// <param param name="line">The 0-based line in the file the entries were read from</param>
        /// <param name="entries">The individual pieces of one line of the CSV</param>
        /// <returns>A T object</returns>
        public abstract T Generate(int line, string[] entries);

        /// <summary>
        /// Generate one line of the CSV from a single T object
        /// </summary>
        /// <param name="t">The object to generate the CSV</param>
        /// <returns>A string that represents t as the appropriate line of the CSV</returns>
        public abstract String GenerateLine(T t);

        /// <summary>
        /// "Callback" called as each line in the CSV file is read
        /// </summary>
        /// <param name="line">The 0-based line in the file</param>
        /// <param name="entries">The individual entries in the row</param>
        public void OnCSVEntry(int line, string[] entries)
        {
            T t = Generate(line, entries);
            Add(t);
        }

        /// <summary>
        /// "Callback" called as an error in the CSV file is detected
        /// </summary>
        /// <param name="message"></param>
        public void OnError(string message)
        {
            string trimmed = message.Trim();
            if (string.IsNullOrEmpty(trimmed) == false)
                Errors.Add(trimmed);
        }

        public void WriteFile()
        {
            CSVWriter writer = new CSVWriter(Filename);
            writer.doWrite(this);
        }

        protected void Init()
        {
            if (Entries.Count == 0)
            {
                CSVReader reader = new CSVReader(Filename);
                reader.doRead(this);
            }
        }

    }
}
