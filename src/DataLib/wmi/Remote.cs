using gov.sandia.sld.common.utilities;
using System;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    /// <summary>
    /// Information needed to connect to a different machine remotely
    /// </summary>
    public class Remote
    {
        private System.Net.IPAddress _ipAddr;

        public string IPAddress { get { return _ipAddr.ToString(); } }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool HasIPAddress { get { return IPAddress.IsIPAddress(); } }
        public bool HasUsernamePassword { get { return string.IsNullOrEmpty(Username) == false && string.IsNullOrEmpty(Password) == false; } }

        public Remote()
        {
            _ipAddr = null;
            Username = Password = string.Empty;
        }

        public Remote(string ip_address)
        {
            _ipAddr = ip_address.GetIPAddress();

            if (_ipAddr == null)
                throw new Exception("Remote: Invalid IP address");

            Username = Password = string.Empty;
        }

        public Remote(string ip_address, string username, string password)
        {
            _ipAddr = ip_address.GetIPAddress();

            if (_ipAddr == null)
                throw new Exception("Remote: Invalid IP address");

            Username = username;
            Password = password;
        }

        public Remote (System.Net.IPAddress addr)
        {
            if (addr == null)
                throw new Exception("Remote: Null IP address");

            _ipAddr = addr;
            Username = Password = string.Empty;
        }

        public Remote(System.Net.IPAddress addr, string username, string password)
        {
            if (addr == null)
                throw new Exception("Remote: Null IP address");

            _ipAddr = addr;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Using the specified namespace, get a ManagementScope object.
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public ManagementScope GetManagementScope(string ns = "CIMV2")
        {
            ManagementScope scope = null;

            if (_ipAddr != null)
            {
                scope = new ManagementScope($@"\\{_ipAddr.ToString()}\root\{ns}");

                if (HasUsernamePassword)
                {
                    scope.Options = new ConnectionOptions() { Username = Username, Password = Password };
                    scope.Connect();
                }
            }
            else
                scope = new ManagementScope($@"root\{ns}");

            return scope;
        }
    }
}
