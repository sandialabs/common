using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.requestresponse
{
    public class IPAddressRequest : Request
    {
        // A list of IP addresses and the name of whatever's at that IP addr
        public List<Tuple<string, string>> IPAddresses { get; set; }
        public List<string> Subnets { get; set; }
        public int NumPingers { get; set; }

        public IPAddressRequest(string name)
            : base(name)
        {
            IPAddresses = new List<Tuple<string, string>>();
            Subnets = new List<string>();
            NumPingers = 8;
        }
    }
}
