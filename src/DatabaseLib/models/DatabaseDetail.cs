using System;

namespace gov.sandia.sld.common.db.models
{
    public class DatabaseDetail
    {
        public int sizeInMB { get; set; }
        public DateTimeOffset timestamp { get; set; }
    }
}
