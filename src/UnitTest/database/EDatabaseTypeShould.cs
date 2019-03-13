using gov.sandia.sld.common.configuration;
using Xunit;

namespace UnitTest
{
    public class EDatabaseTypeShould
    {
        [Fact]
        public void CreateFromStringProperly()
        {
            EDatabaseType type = "SqlServer".GetDatabaseType();
            Assert.Equal(EDatabaseType.SqlServer, type);

            type = "Oracle".GetDatabaseType();
            Assert.Equal(EDatabaseType.Oracle, type);

            type = "Postgres".GetDatabaseType();
            Assert.Equal(EDatabaseType.Postgres, type);

            type = "sQLsERVER".GetDatabaseType();
            Assert.Equal(EDatabaseType.SqlServer, type);

            type = "unknown".GetDatabaseType();
            Assert.Equal(EDatabaseType.Unknown, type);

            type = "lsjfioaerkjaosdfjlkserjoivuahg".GetDatabaseType();
            Assert.Equal(EDatabaseType.Unknown, type);

            type = string.Empty.GetDatabaseType();
            Assert.Equal(EDatabaseType.Unknown, type);
        }
    }
}
