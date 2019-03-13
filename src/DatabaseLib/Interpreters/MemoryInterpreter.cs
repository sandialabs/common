using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.wmi;
using System;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db.interpreters
{
    public class MemoryInterpreter : BaseInterpreter, IDataInterpreter
    {
        public void Interpret(Data d, SQLiteConnection conn)
        {
            if (d.Type != ECollectorType.Memory)
                return;

            if (d is GenericData<MemoryUsage>)
            {
                GenericData<MemoryUsage> data = d as GenericData<MemoryUsage>;

                MemoryLowAlert low_level = new MemoryLowAlert();
                MemoryCriticallyLowAlert critically_low_level = new MemoryCriticallyLowAlert();
                double low = low_level.GetValueAsDouble(conn) ?? 80.0;
                double critically_low = critically_low_level.GetValueAsDouble(conn) ?? 90.0;

                ulong capacity = data.Generic.CapacityNum;
                ulong free = data.Generic.FreeNum;
                ulong percent_used = (ulong)((double)free / (double)capacity * 100.0 + 0.5);
                string message = $"{percent_used} %";

                MemoryStatus mem_status = new MemoryStatus(low, critically_low);
                EStatusType status = mem_status.GetStatus(data.Generic.UsedNum, capacity);

                long device_id = GetDeviceID(d, conn);
                if (device_id >= 0)
                    SetDeviceStatus(device_id, status, MemoryStatus.Types, message, conn);
            }
            else
                throw new Exception("MemoryInterpreter: data type is wrong");
        }
    }
}
