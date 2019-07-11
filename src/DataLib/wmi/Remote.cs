using gov.sandia.sld.common.utilities;
using System.Management;

namespace gov.sandia.sld.common.data.wmi
{
    /// <summary>
    /// Information needed to connect to a different machine remotely
    /// </summary>
    public class Remote
    {
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool HasIPAddress { get { return IPAddress.IsIPAddress(); } }
        public bool HasUsernamePassword { get { return string.IsNullOrEmpty(Username) == false && string.IsNullOrEmpty(Password) == false; } }

        public Remote()
        {
            IPAddress = Username = Password = string.Empty;
        }

        public Remote(string ip_address)
        {
            IPAddress = ip_address;
            Username = Password = string.Empty;
        }

        public Remote(string ip_address, string username, string password)
        {
            IPAddress = ip_address;
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

            if (HasIPAddress)
            {
                scope = new ManagementScope(@"\\" + IPAddress + @"\root\" + ns);

                if (HasUsernamePassword)
                {
                    scope.Options = new ConnectionOptions() { Username = Username, Password = Password };
                    scope.Connect();
                }
            }
            else
                scope = new ManagementScope(@"root\" + ns);

            return scope;
        }
    }
}
