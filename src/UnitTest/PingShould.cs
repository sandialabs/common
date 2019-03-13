using Xunit;
using gov.sandia.sld.common.data;
using System.Net;

namespace UnitTest
{
    public class PingShould
    {
        [Fact]
        public void PingOK()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            PingResult pingResult = new PingResult()
            {   Address = ipAddress,
                Name = "Test Server",
                AvgTime = 1,
                IsPingable = true,
                MAC = "00-90-C2-D8-7E-CD"
            };

            Assert.Equal(ipAddress, pingResult.Address);
            Assert.Equal("Test Server", pingResult.Name);
            Assert.Equal(1, pingResult.AvgTime);
            Assert.True(pingResult.IsPingable);
            Assert.Equal("00-90-C2-D8-7E-CD", pingResult.MAC);

        }

        //[Fact]
        //public void DeserializeJSONPingOK()
        //{
            //string json =
            //    "{\"Value\":{\"Address\":\"192.168.100.2\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"Switch A\",\"MAC\":\"F4-8E-38-4F-27-89\"},{\"Address\":\"192.168.100.12\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"Server A\",\"MAC\":\"3C-A8-2A-15-C7-58\"},{\"Address\":\"192.168.100.66\",\"IsPingable\":false,\"AvgTime\":1000,\"Name\":\"Workstation A\",\"MAC\":\"\"},{\"Address\":\"192.168.100.67\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"Workstation B\",\"MAC\":\"40-A8-F0-AC-76-34\"},{\"Address\":\"192.168.100.131\",\"IsPingable\":true,\"AvgTime\":1,\"Name\":\"L001\",\"MAC\":\"00-90-C2-D8-7E-CD\"},{\"Address\":\"192.168.100.132\",\"IsPingable\":true,\"AvgTime\":1,\"Name\":\"L002\",\"MAC\":\"00-90-C2-DA-68-3C\"},{\"Address\":\"192.168.100.133\",\"IsPingable\":true,\"AvgTime\":1,\"Name\":\"L003\",\"MAC\":\"00-90-C2-C4-C5-E5\"},{\"Address\":\"192.168.100.134\",\"IsPingable\":true,\"AvgTime\":1,\"Name\":\"L004\",\"MAC\":\"00-90-C2-C4-DD-CC\"},{\"Address\":\"192.168.100.135\",\"IsPingable\":true,\"AvgTime\":1,\"Name\":\"L005\",\"MAC\":\"00-90-C2-F9-1F-E0\"},{\"Address\":\"192.168.100.162\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L003 Camera 1\",\"MAC\":\"10-4F-A8-5A-81-6E\"},{\"Address\":\"192.168.100.163\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L003 Camera 2\",\"MAC\":\"10-4F-A8-5A-82-1C\"},{\"Address\":\"192.168.100.166\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L002 Camera 1\",\"MAC\":\"10-4F-A8-5A-81-C3\"},{\"Address\":\"192.168.100.167\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L002 Camera 2\",\"MAC\":\"10-4F-A8-5A-81-64\"},{\"Address\":\"192.168.100.170\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L001 Camera 1\",\"MAC\":\"10-4F-A8-5A-82-40\"},{\"Address\":\"192.168.100.171\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L001 Camera 2\",\"MAC\":\"10-4F-A8-5A-81-B6\"},{\"Address\":\"192.168.100.174\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L004 Camera 1\",\"MAC\":\"10-4F-A8-5A-81-57\"},{\"Address\":\"192.168.100.175\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L004 Camera 2\",\"MAC\":\"10-4F-A8-5A-82-17\"},{\"Address\":\"192.168.100.178\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L005 Camera 1\",\"MAC\":\"10-4F-A8-5A-81-BA\"},{\"Address\":\"192.168.100.179\",\"IsPingable\":true,\"AvgTime\":0,\"Name\":\"L005 Camera 2\",\"MAC\":\"10-4F-A8-5A-82-09\"} }";
            //var definition = new { Value = new Dictionary<string, PingResult>() };
            //var value = JsonConvert.DeserializeAnonymousType(json, definition);

            //Assert.NotNull(value);
            //Assert.NotNull(value.Value);
            //Assert.NotEmpty(value.Value);

            //bool exists = value.Value.TryGetValue("COMMONCLI", out PingResult commoncli);
            //Assert.True(exists);

            //IPAddress ipAddress = IPAddress.Parse("192.168.100.2");

            //Assert.Equal(ipAddress, commoncli.Address);
            //Assert.True(true, commoncli.IsPingable);
            //Assert.Equal((ulong)17719296, commoncli.WorkingSetNum);
            //Assert.Equal((ulong)4505600, commoncli.WorkingSetPrivateNum);
            //Assert.Equal((ulong)15864, commoncli.PoolNonpagedBytesNum);
            //Assert.Equal((ulong)251944, commoncli.PoolPagedBytesNum);
            //Assert.Equal((ulong)14340096, commoncli.PrivateBytesNum);
            //Assert.Equal("17719296", commoncli.Memory);
            //Assert.Equal((ulong)17719296, commoncli.MemoryNum);
            //Assert.Equal(16.8984375, commoncli.MemoryInMBNum, 1);
        //}
    }
}
