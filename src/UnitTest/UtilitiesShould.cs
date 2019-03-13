using gov.sandia.sld.common.utilities;
using System;
using System.Net;
using Xunit;

namespace UnitTest
{
    public class UtilitiesShould
    {
        [Fact]
        public void IsIPShould()
        {
            string ipTrue = "192.168.0.1";
            string ipFalse = "Cat";
            string ipFalse2 = "123423.3243.324.23";


            bool isValidIP = gov.sandia.sld.common.utilities.Extensions.IsIPAddress(ipTrue);
            bool isNotValidIP = gov.sandia.sld.common.utilities.Extensions.IsIPAddress(ipFalse);
            bool isNotValidIP2 = gov.sandia.sld.common.utilities.Extensions.IsIPAddress(ipFalse2);

            Assert.True(isValidIP);
            Assert.False(isNotValidIP);
            Assert.False(isNotValidIP2);
        }

        [Fact]
        public void IPEqualsIPShould()
        {
            IPAddress ip1 = IPAddress.Parse("192.168.0.1");
            IPAddress ip2 = IPAddress.Parse("192.168.0.1");
            IPAddress ip3 = IPAddress.Parse("192.168.0.2");

            IPAddressComparer comparer = new IPAddressComparer();
            //Test equals method in IPAddressComparer
            bool isAddressEqual = comparer.Equals(ip1, ip2);
            bool isAddressEqual2 = comparer.Equals(ip2, ip3);

            Assert.True(isAddressEqual);
            Assert.False(isAddressEqual2);

            //Test compare method in IPAddressComparer
            int compare = comparer.Compare(ip1, ip3);
            Assert.Equal(-1, compare);

        }
    }
}
