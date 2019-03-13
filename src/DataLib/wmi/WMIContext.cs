using System.Collections.Generic;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    public class WMIContext
    {
        public string Class { get; protected set; }
        public string Properties { get; protected set; }
        public List<string> PropertiesList { get; protected set; }
        public string Where { get; set; }
        public Remote RemoteInfo { get; protected set; }

        public WMIContext(string c, string p, Remote remote_info, string w = "")
        {
            Class = c;
            Properties = p;
            Where = w;
            RemoteInfo = remote_info;

            PropertiesList = new List<string>(Properties.Split(',')).ConvertAll<string>(x => x.Trim());
        }

        /// <summary>
        /// Using the specified namespace, get a ManagementScope object. If RemoteInfo is specified,
        /// things will be handled properly to connect to the remote system.
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public ManagementScope GetManagementScope(string ns = "CIMV2")
        {
            ManagementScope scope = RemoteInfo!= null ? RemoteInfo.GetManagementScope(ns) : new ManagementScope(@"root\" + ns);
            return scope;
        }
    }
}
