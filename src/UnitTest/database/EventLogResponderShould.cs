using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.responders;
using gov.sandia.sld.common.requestresponse;
using gov.sandia.sld.common.utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Management;
using Xunit;

namespace UnitTest.database
{
    public class EventLogResponderShould
    {
        [Theory]
        [InlineData("system", SystemErrorsInfoRequest.EType.System)]
        [InlineData("application", SystemErrorsInfoRequest.EType.Application)]
        public void ConvertProperly(string section, SystemErrorsInfoRequest.EType type)
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                Initializer init = new Initializer(null);
                init.Initialize(db);

                RequestBus bus = new RequestBus();
                EventLogResponder responder = new EventLogResponder() { DB = db };
                bus.Subscribe(responder);

                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    string orig_path = $"machine_name.{section}.max_record_number";
                    string new_path = $"machine_name.{section}.max_event_log";
                    ulong record_num = 234567;

                    Attribute attr = new Attribute();
                    attr.Set(orig_path, record_num.ToString(), conn);

                    SystemErrorsInfoRequest req = new SystemErrorsInfoRequest("machine_name", type);
                    bus.MakeRequest(req);
                    Assert.True(req.IsHandled);
                    Assert.True(req.LogData.MaxRecordNumber.HasValue);
                    Assert.Equal(record_num, req.LogData.MaxRecordNumber.Value);

                    string x = attr.Get(orig_path, conn);
                    Assert.True(string.IsNullOrEmpty(x));

                    x = attr.Get(new_path, conn);
                    Assert.False(string.IsNullOrEmpty(x));

                    EventLogData eld = JsonConvert.DeserializeObject<EventLogData>(x);
                    Assert.NotNull(eld);
                    Assert.True(eld.MaxRecordNumber.HasValue);
                    Assert.Equal(record_num, eld.MaxRecordNumber.Value);
                }
            }
        }

        [Theory]
        [InlineData("system", SystemErrorsUpdateRequest.EType.System)]
        [InlineData("application", SystemErrorsUpdateRequest.EType.Application)]
        public void HandleUpdateRequestProperly(string section, SystemErrorsUpdateRequest.EType type)
        {
            using (FileDeleter fd = new FileDeleter(Extensions.GetTempDBFile()))
            {
                Database db = new Database(new Context(fd.Fi));
                Initializer init = new Initializer(null);
                init.Initialize(db);

                RequestBus bus = new RequestBus();
                EventLogResponder responder = new EventLogResponder() { DB = db };
                bus.Subscribe(responder);

                using (SQLiteConnection conn = db.Connection)
                {
                    conn.Open();

                    string new_path = $"machine_name.{section}.max_event_log";

                    System.DateTime dt = GenerateTime(2019, 2, 20, 12, 23, 34, 456789);
                    EventLogData eld = new EventLogData();
                    eld.Insert(dt, 22334455);
                    eld.Insert(dt, 22334456);

                    SystemErrorsUpdateRequest req = new SystemErrorsUpdateRequest("machine_name", type);
                    req.LogData.Assign(eld);
                    bus.MakeRequest(req);
                    Assert.True(req.IsHandled);

                    Attribute attr = new Attribute();
                    string x = attr.Get(new_path, conn);
                    Assert.False(string.IsNullOrEmpty(x));

                    EventLogData eld2 = JsonConvert.DeserializeObject<EventLogData>(x);
                    Assert.NotNull(eld);
                    Assert.False(eld2.MaxRecordNumber.HasValue);
                    Assert.Equal(dt, eld2.MaxDate);

                    List<ulong> record_numbers = eld2.MaxDateToRecordNumbers[dt];
                    Assert.NotNull(record_numbers);
                    Assert.Equal(2, record_numbers.Count);
                    // Sort just in case they got put in the list in a different order
                    record_numbers.Sort();
                    Assert.Equal((ulong)22334455, record_numbers[0]);
                    Assert.Equal((ulong)22334456, record_numbers[1]);
                }
            }
        }

        private static System.DateTime GenerateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            return ManagementDateTimeConverter.ToDateTime($"{year:0000}{month:00}{day:00}{hour:00}{minute:00}{second:00}.{millisecond:000000}-000");
        }
    }
}
