using System;

namespace gov.sandia.sld.common.configuration
{
    public enum EDatabaseType
    {
        Oracle,
        SqlServer,
        Postgres,

        Unknown = -1
    }

    public static class EDatabaseTypeExtensions
    {
        public static EDatabaseType GetDatabaseType(this string type_str)
        {
            if (Enum.TryParse<EDatabaseType>(type_str, true, out EDatabaseType type))
                return type;
            else
                return EDatabaseType.Unknown;
        }
    }
}
