using Xunit;
using gov.sandia.sld.common.data.wmi;

namespace UnitTest.wmi
{
    public class ApplicationShould
    {
        [Fact]
        public void ApplicationOK()
        {
            ApplicationInfo application = new ApplicationInfo()
            {
                Name = "Test Name",
                Version = "1.0.1"
            };

            Assert.Equal("Test Name", application.Name);
            Assert.Equal("1.0.1", application.Version);
        }
    }
}
