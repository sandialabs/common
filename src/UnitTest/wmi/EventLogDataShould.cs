using gov.sandia.sld.common.requestresponse;
using System;
using System.Collections.Generic;
using System.Management;
using Xunit;

namespace UnitTest.wmi
{
    public class EventLogDataShould
    {
        [Fact]
        public void HaveDefaultNullMaxRecordNumber()
        {
            EventLogData eld = new EventLogData();
            Assert.Null(eld.MaxRecordNumber);
        }

        [Fact]
        public void ProperlyHandleMaxRecordNumber()
        {
            EventLogData eld = new EventLogData() { MaxRecordNumber = 123456789 };

            Assert.True(eld.ContainsRecordNumber(123456789));
            Assert.False(eld.ContainsRecordNumber(2222222222));
        }

        [Fact]
        public void HaveAssignWorkProperly()
        {
            EventLogData eld = new EventLogData();
            eld.MaxRecordNumber = 2;
            Assert.True(eld.MaxRecordNumber.HasValue);
            Assert.Equal((ulong)2, eld.MaxRecordNumber.Value);

            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 56, 789), 22334455);
            Assert.True(eld.ContainsRecordNumber(22334455));

            EventLogData eld2 = new EventLogData();
            eld2.Assign(eld);

            Assert.True(eld2.MaxRecordNumber.HasValue);
            Assert.Equal((ulong)2, eld2.MaxRecordNumber.Value);
            Assert.True(eld2.ContainsRecordNumber(22334455));
        }

        [Fact]
        public void HaveContainsRecordNumberWorkProperly()
        {
            EventLogData eld = new EventLogData();
            ulong start_record_num = 22334455;
            ulong record_num = start_record_num;
            int start_ms = 234567;
            int ms = start_ms;
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);

            record_num = start_record_num;
            Assert.True(eld.ContainsRecordNumber(record_num++));
            Assert.True(eld.ContainsRecordNumber(record_num++));
            Assert.True(eld.ContainsRecordNumber(record_num++));
            Assert.False(eld.ContainsRecordNumber(record_num++));
            Assert.False(eld.ContainsRecordNumber(start_record_num - 1));
        }

        [Fact]
        public void HandleMaxDateProperly()
        {
            EventLogData eld = new EventLogData();

            Assert.Null(eld.MaxDate);

            ulong start_record_num = 22334455;
            ulong record_num = start_record_num;
            int start_ms = 234567;
            int ms = start_ms;
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);

            Assert.NotNull(eld.MaxDate);
            Assert.Equal(GenerateTime(2019, 2, 20, 12, 34, 45, ms - 1), eld.MaxDate.Value);
        }

        [Fact]
        public void CleanupProperly()
        {
            EventLogData eld = new EventLogData();
            eld.MaxRecordNumber = 2;
            ulong start_record_num = 22334455;
            ulong record_num = start_record_num;
            int start_ms = 234567;
            int ms = start_ms;
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);
            eld.Insert(GenerateTime(2019, 2, 20, 12, 34, 45, ms++), record_num++);

            Assert.Equal(3, eld.MaxDateToRecordNumbers.Keys.Count);
            Assert.NotNull(eld.MaxRecordNumber);

            eld.Cleanup();

            // After cleanup, only the most-recent time should be in there, and the MaxRecordNumber
            // should be null
            Assert.Single(eld.MaxDateToRecordNumbers.Keys);
            Assert.Null(eld.MaxRecordNumber);
        }

        [Fact]
        public void HandleMultipleRecordNumbersAtTheSameTimeProperly()
        {
            EventLogData eld = new EventLogData();
            ulong start_record_num = 22334455;
            ulong record_num = start_record_num;
            DateTime dt = GenerateTime(2019, 2, 20, 12, 34, 45, 234567);
            eld.Insert(dt, record_num++);
            eld.Insert(dt, record_num++);
            eld.Insert(dt, record_num++);

            Assert.Single(eld.MaxDateToRecordNumbers.Keys);

            List<ulong> record_numbers = eld.MaxDateToRecordNumbers[dt];
            Assert.NotNull(record_numbers);
            Assert.Equal(3, record_numbers.Count);
        }

        private static DateTime GenerateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
        {
            return ManagementDateTimeConverter.ToDateTime($"{year:0000}{month:00}{day:00}{hour:00}{minute:00}{second:00}.{millisecond:000000}-000");
        }
    }
}
