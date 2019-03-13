using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.data.database
{
    public interface IDatabaseCollector
    {
        Tuple<Dictionary<string, ulong>, bool> GetData(string connection_string);
    }
}
