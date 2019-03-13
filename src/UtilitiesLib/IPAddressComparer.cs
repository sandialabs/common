using System;
using System.Collections.Generic;
using System.Net;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Used to sort IPAddress objects
    /// </summary>
    public class IPAddressComparer : IComparer<IPAddress>, IEqualityComparer<IPAddress>
    {
        public int Compare(IPAddress a, IPAddress b)
        {
            int a_addr = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(a.GetAddressBytes(), 0));
            int b_addr = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(b.GetAddressBytes(), 0));
            //Trace.WriteLine(string.Format("{0} - {1} == {2}", a_addr, b_addr, a_addr - b_addr));
            return a_addr - b_addr;
        }

        public bool Equals(IPAddress x, IPAddress y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(IPAddress obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
