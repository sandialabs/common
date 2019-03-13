using gov.sandia.sld.common.configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data
{
    public delegate void DataAcquired(CollectedData cd);

    /// <summary>
    /// When data is collected, it is put in Data, or a child of Data, and passed
    /// around that way.
    /// </summary>
    public class Data
    {
        [JsonIgnore]
        public DataCollectorContext Context { get; private set; }
        [JsonIgnore]
        public long ID { get { return Context.ID.ID; } set { Context.ID.ID = value; } }
        [JsonIgnore]
        public string Name { get { return Context.ID.Name; } set { Context.ID.Name = value; } }
        public string Value { get { return m_value; } set { m_value = value; /*m_data_collected = true;*/ } }
        [JsonIgnore]
        public ECollectorType Type { get { return Context.Type; } set { Context.Type = value; } }
        [JsonIgnore]
        public DateTimeOffset CollectedAt { get; set; }

        [JsonIgnore]
        public UInt64? ValueAsUInt64
        {
            get
            {
                if (string.IsNullOrEmpty(Value) == false)
                {
                    if (UInt64.TryParse(Value, out ulong u))
                        return u;
                }

                return null;
            }
        }

        public Data(DataCollectorContext context)
        {
            Context = context;
            m_value = string.Empty;
            CollectedAt = DateTimeOffset.Now;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, Value ?? "null");
        }

        private string m_value;

        public static double AsMegabytes(UInt64 bytes)
        {
            return AsPower(bytes, 2);
        }

        public static double AsGigabytes(UInt64 bytes)
        {
            return AsPower(bytes, 3);
        }

        public static double AsPower(UInt64 value, uint power)
        {
            double p = Math.Pow(1024, (double)power);
            return (double)value / p;
        }
    }

    public class GenericData<T> : Data
    {
        [JsonProperty("Value")]
        public T Generic { get; set; }

        public GenericData(DataCollectorContext context, T t) : base(context)
        {
            Generic = t;
        }
    }

    public class ListData<T> : Data
    {
        [JsonProperty("Value")]
        public List<T> Data { get; private set; }

        public ListData(DataCollectorContext context)
            : base(context)
        {
            Data = new List<T>();
        }
    }

    public class ListStringData : ListData<string>
    {
        public enum Options
        {
            NoDuplicates = 0x00000001,
            KeepSorted = 0x00000002,
            IgnoreCase = 0x00000004,
        }

        public ListStringData(DataCollectorContext context, Options options)
            : base(context)
        {
            m_options = options;
        }

        public bool Add(string s)
        {
            bool add = true;
            bool ignore_case = OptionEnabled(Options.IgnoreCase);
            StringComparer comparer = ignore_case ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;

            if (OptionEnabled(Options.NoDuplicates))
            {
                // Don't add it if it already exists in the list

                if (OptionEnabled(Options.KeepSorted))
                {
                    // It's sorted, so we can do a binary search instead of a linear search for
                    // better performance. BinarySearch returns a negative value if it wasn't found,
                    // so add if it's negative.
                    add = Data.BinarySearch(s, comparer) < 0;
                }
                else
                {
                    // Exists will return true if it's in the list, so we want to invert that
                    // (i.c. add if it doesn't exist)
                    add = !Data.Exists(e => comparer.Compare(s, e) == 0);
                }
            }

            if (add)
            {
                Data.Add(s);

                if (OptionEnabled(Options.KeepSorted))
                    Data.Sort(comparer);
            }

            return add;
        }

        public override string ToString()
        {
            string value = string.Empty;
            Data.ForEach(d =>
            {
                if (string.IsNullOrEmpty(value) == false)
                    value += ",";
                value += d;
            });
            return string.Format("{0}: {1}", Name, value);
        }

        private bool OptionEnabled(Options option)
        {
            return (m_options & option) != 0;
        }

        private Options m_options;
    }

    public class GenericDictionaryData<T> : Data
    {
        [JsonProperty("Value")]
        public Dictionary<string, T> Data { get; set; }

        public GenericDictionaryData(DataCollectorContext context)
            : base(context)
        {
            Data = new Dictionary<string, T>();
        }
    }

    public class DictionaryData : GenericDictionaryData<string>
    {
        public DictionaryData(DataCollectorContext context)
            : base(context)
        {
        }
    }

    public class CollectedData
    {
        public DataCollectorContext Context { get; private set; }
        public bool DataIsCollected { get; set; }
        public List<Data> D { get; private set; }
        public string Message { get; set; }

        public CollectedData(DataCollectorContext context, bool collected, IEnumerable<Data> datalist)
        {
            Context = context;
            DataIsCollected = collected;
            D = new List<Data>(datalist);
        }

        public CollectedData(DataCollectorContext context, bool collected, Data d = null)
        {
            Context = context;
            DataIsCollected = collected;
            if (d == null)
                D = new List<Data>();
            else
                D = new List<Data>(new Data[] { d });
        }

        public void SetMessage(Exception e)
        {
            Message = string.Empty;
            while(e != null)
            {
                if (string.IsNullOrEmpty(Message) == false)
                    Message += "\n";
                Message += e.Message;

                e = e.InnerException;
            }
        }
    }
}
