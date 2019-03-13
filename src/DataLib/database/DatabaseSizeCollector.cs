using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.database
{
    public class DatabaseSizeCollector : DataCollector
    {
        public bool IsServer { get; private set; }
        public EDatabaseType DBType { get; set; }
        public string ConnectionString { get; set; }
        public IDatabaseCollectorFactory Factory { get; private set; }

        public DatabaseSizeCollector(CollectorID id, bool is_server, IDatabaseCollectorFactory factory)
            : base(new DataCollectorContext(id, ECollectorType.DatabaseSize))
        {
            IsServer = is_server;
            Factory = factory;
        }

        public override CollectedData OnAcquire()
        {
            Tuple<Dictionary<string, ulong>, bool> dict = this.QueryDatabase();

            // Don't report the COMMON DB unless we're the server
            if (IsServer)
            {
                COMMONDBSizeRequest common_db_request = new COMMONDBSizeRequest();
                RequestBus.Instance.MakeRequest(common_db_request);
                if (common_db_request.IsHandled)
                    dict.Item1[common_db_request.Filename] = common_db_request.SizeInMB;
            }

            ListData<Dictionary<string, string>> data = new ListData<Dictionary<string, string>>(Context);
            foreach (KeyValuePair<string, ulong> database in dict.Item1)
            {
                Dictionary<string, string> size = new Dictionary<string, string>
                {
                    ["Name"] = database.Key,
                    ["Size"] = database.Value.ToString()
                };

                data.Data.Add(size);
            }

            return new CollectedData(Context, dict.Item2, data);
        }

        protected Tuple<Dictionary<string, ulong>, bool> QueryDatabase()
        {
            GetDBInfo();

            Tuple<Dictionary<string, ulong>, bool> data = Tuple.Create(new Dictionary<string, ulong>(), true);
            IDatabaseCollector collector = Factory.Create(DBType);
            if (collector != null)
                data = collector.GetData(ConnectionString);

            return data;
        }

        protected void GetDBInfo()
        {
            DatabaseTypeRequest request = new DatabaseTypeRequest("Database Size Request", Context.ID.Name);
            RequestBus.Instance.MakeRequest(request);
            if (request.IsHandled)
            {
                DBType = request.DatabaseType;
                ConnectionString = request.ConnectionString;
            }
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            ListData<Dictionary<string, string>> d = new ListData<Dictionary<string, string>>(context);
            var definition = new { Value = new List<Dictionary<string, string>>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d.Data.AddRange(data.Value);
            return d;
        }
    }
}
