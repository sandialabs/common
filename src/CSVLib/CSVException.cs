using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSVLib
{
    public class CSVException : Exception
    {
        public int Field { get; private set; }
        public int Line { get; private set; }
        public string Value { get; private set; }

        public CSVException(int field, int line, string value, string message)
            : base(string.Format("{0}: value {1}, field {2}, line {3}", message, value, field, line))
        {
            Field = field;
            Line = line;
            Value = value;
        }
    }
}
