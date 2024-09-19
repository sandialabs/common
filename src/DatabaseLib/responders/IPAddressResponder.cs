using gov.sandia.sld.common.requestresponse;
using System;
using System.Linq;
using System.Collections.Generic;
using gov.sandia.sld.common.utilities;
using System.Data.SQLite;
using gov.sandia.sld.common.db.models;

namespace gov.sandia.sld.common.db.responders
{
    public class IPAddressResponder : IResponder
    {
        public void HandleRequest(IRequest request)
        {
            if (request is IPAddressRequest)
            {
                IPAddressRequest ip_request = request as IPAddressRequest;
                Database db = new Database();

                try
                {
                    using (SQLiteConnection conn = db.Connection)
                    {
                        conn.Open();

                        List<DeviceInfo> devices = db.GetDevices().Where(d => d.ipAddress.IsIPAddress()).ToList();
                        devices.ForEach(d => ip_request.IPAddresses.Add(Tuple.Create(d.ipAddress.Trim(), d.name)));

                        PingExtras pe = new PingExtras();
                        string extra_pings = pe.GetValue(conn);
                        if (extra_pings != null)
                        {
                            string[] extras = extra_pings.Split(',');
                            foreach (string extra in extras)
                            {
                                string e = extra.Trim();
                                if (e.IsIPAddress())
                                    ip_request.IPAddresses.Add(Tuple.Create(e, string.Empty));
                            }
                        }

                        PingSubnets ps = new PingSubnets();
                        string subnet = ps.GetValue(conn);
                        if (subnet != null)
                        {
                            string[] subnets = subnet.Split(',');
                            foreach (string s in subnets)
                            {
                                string s2 = s.Trim();
                                if (s2.IsIPAddress())
                                    ip_request.Subnets.Add(s2);
                            }
                        }

                        PingNumPingers pn = new PingNumPingers();
                        int? num_pingers = pn.GetValueAsInt(conn);
                        if (num_pingers.HasValue)
                            ip_request.NumPingers = num_pingers.Value;

                        ip_request.Handled();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
