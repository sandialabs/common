using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using gov.sandia.sld.common.requestresponse;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;

// http://stackoverflow.com/a/4042887/706747

namespace gov.sandia.sld.common.data
{
    public class PingResult
    {
        [JsonConverter(typeof(IPAddressConverter))]
        public IPAddress Address { get; set; }
        public bool IsPingable { get; set; }
        public long AvgTime { get; set; }
        public string Name { get; set; }
        public string MAC { get; set; }

        public PingResult()
        {
            Address = null;
            IsPingable = false;
            AvgTime = 0;
            Name = MAC = string.Empty;
        }
    }

    public class IPAddressConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse(JToken.Load(reader).ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken.FromObject(value.ToString()).WriteTo(writer);
        }
    }

    public static class PingExtensions
    {
        public static PingResult PingIt(this IPAddress addr, string name)
        {
            PingResult result = new PingResult() { Address = addr, Name = name };
            ILog log = LogManager.GetLogger(typeof(PingExtensions));

            try
            {
                if(string.IsNullOrEmpty(name))
                    log.DebugFormat("Pinging {0}", addr.ToString());
                else
                    log.DebugFormat("Pinging {0} ({1})", addr.ToString(), name);

                long ms = 0;
                uint count = 0;
                long repeat = 5;
                int timeout = 1000; // milliseconds
                //Stopwatch watch = Stopwatch.StartNew();

                using (Ping pinger = new Ping())
                {
                    for (int i = 0; i < repeat; ++i)
                    {
                        PingReply reply = pinger.Send(addr, timeout);
                        if (reply.Status == IPStatus.Success)
                        {
                            ++count;
                            ms += reply.RoundtripTime;

                            log.DebugFormat("Ping {0} success in {1} ms", addr.ToString(), reply.RoundtripTime);
                        }
                        else
                        {
                            ms += timeout;

                            log.DebugFormat("Ping {0} failure {1}", addr.ToString(), reply.Status.ToString());
                        }
                    }
                }

                //Trace.WriteLine(string.Format("Pinging {0} took {1} ms", addr.ToString(), watch.ElapsedMilliseconds));
                log.DebugFormat("Ping {0}, count {1}, total ms {2}", addr.ToString(), count, ms);

                // If we average more than half, consider it good
                result.IsPingable = ((double)count / (double)repeat) >= 0.5f;
                result.AvgTime = ms / repeat;
            }
            catch (Exception e)
            {
                log.ErrorFormat("Error in PingIt: {0}", addr.ToString());
                log.Error(e);
            }

            return result;
        }
    }

    /// <summary>
    /// The DataCollector that collects ping data for the local network.
    /// </summary>
    public class PingCollector : DataCollector
    {
        public PingCollector(CollectorID id)
            : base(new DataCollectorContext(id, ECollectorType.Ping))
        {
        }

        public override CollectedData OnAcquire()
        {
            // Find the IP addresses to ping. This will typically provide the IP addresses
            // of the devices being monitored, and can also provide a subnet to ping, and
            // also any extra addresses to ping.
            IPAddressRequest request = new IPAddressRequest("PingCollector");
            RequestBus.Instance.MakeRequest(request);
            if (request.IsHandled == false)
                return null;

            ListData<PingResult> d = new ListData<PingResult>(Context);
            List<Tuple<IPAddress, string>> to_ping = new List<Tuple<IPAddress, string>>();
            Dictionary<string, string> ip_to_name_map = new Dictionary<string, string>();
            request.IPAddresses.ForEach(i => ip_to_name_map[i.Item1] = i.Item2);

            // See if a full subnet ping was requested
            foreach(string s in request.Subnets)
            {
                if (IPAddress.TryParse(s, out IPAddress subnet))
                {
                    byte[] ping_addr = subnet.GetAddressBytes();

                    // Collect all the pingable IP addresses on the specified subnet.
                    // 0 and 255 are reserved, so no need to ping them.
                    for (byte i = 1; i < 255; ++i)
                    {
                        ping_addr[3] = i;
                        IPAddress addr = new IPAddress(ping_addr);

                        // Get the name of the device, if we happen to know it
                        string name = string.Empty;
                        ip_to_name_map.TryGetValue(addr.ToString(), out name);

                        to_ping.Add(Tuple.Create(addr, name));
                    }
                }
            }

            // Now put in the other ip addresses that are being monitored.
            // Add them to the list of IPs to ping.
            foreach(Tuple<string, string> ip in request.IPAddresses)
            {
                try
                {
                    if (IPAddress.TryParse(ip.Item1, out IPAddress addr))
                        to_ping.Add(Tuple.Create(addr, ip.Item2));
                }
                catch (Exception)
                {
                }
            }

            // Remove any duplictes IP addresses that might have gotten in there
            IPAddressComparer c = new IPAddressComparer();
            to_ping.Sort((a, b) => c.Compare(a.Item1, b.Item1));
            to_ping = to_ping.Distinct().ToList();

            // Create some # of threads for concurrent pinging. The number of threads
            // will be around request.NumPingers.
            ManualResetEvent reset_event = new ManualResetEvent(false);
            int thread_count = 0;
            List<Thread> threads = new List<Thread>();
            List<List<Tuple<IPAddress, string>>> chunks = to_ping.ChunkBy(to_ping.Count / request.NumPingers);
            foreach (List<Tuple<IPAddress, string>> addrs in chunks)
            {
                Thread t = new Thread(
                    () =>
                    {
                        Stopwatch watch = Stopwatch.StartNew();

                        List<Tuple<IPAddress, string>> local_addrs = addrs;
                        foreach (Tuple<IPAddress, string> addr in local_addrs)
                        {
                            PingResult result = addr.Item1.PingIt(addr.Item2);
                            lock (d.Data)
                                d.Data.Add(result);
                        }

                        Trace.WriteLine(string.Format("Pinging {0} addrs took {1} ms", local_addrs.Count, watch.ElapsedMilliseconds));

                        if (Interlocked.Decrement(ref thread_count) <= 0)
                            reset_event.Set();
                    });
                threads.Add(t);
            }

            if (threads.Count > 0)
            {
                try
                {
                    thread_count = threads.Count;
                    threads.ForEach(t => t.Start());

                    reset_event.WaitOne();
                }
                catch (Exception)
                {
                }
                finally
                {
                }

                Dictionary<string, string> ip_to_mac = GetIPAddrsAndMacAddresses();
                foreach(PingResult pr in d.Data)
                {
                    string ip = pr.Address.ToString();
                    if (ip_to_mac.TryGetValue(ip, out string mac))
                        pr.MAC = mac;
                }
            }

            d.Data.Sort((a, b) => c.Compare(a.Address, b.Address));
            return new CollectedData(Context, true, d);
        }

        public static Data Create(DataCollectorContext context, string value)
        {
            ListData<PingResult> d = new ListData<PingResult>(context);
            var definition = new { Value = new List<PingResult>() };
            var data = JsonConvert.DeserializeAnonymousType(value, definition);
            if (data != null)
                d.Data.AddRange(data.Value);
            return d;
        }

        /// <summary>
        /// Adapted from http://stackoverflow.com/a/19244196/706747
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetIPAddrsAndMacAddresses()
        {
            Dictionary<string, string> ip_to_mac_map = new Dictionary<string, string>();
            try
            {
                System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
                pProcess.StartInfo.FileName = "arp";
                pProcess.StartInfo.Arguments = "-a ";
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.Start();
                string cmdOutput = pProcess.StandardOutput.ReadToEnd();
                string pattern = @"(?<ip>([0-9]{1,3}\.?){4})\s*(?<mac>([a-f0-9]{2}-?){6})";

                foreach (Match m in Regex.Matches(cmdOutput, pattern, RegexOptions.IgnoreCase))
                {
                    string ip = m.Groups["ip"].Value;
                    string mac = m.Groups["mac"].Value.ToUpper();

                    ip_to_mac_map[ip] = mac;
                }
            }
            catch (Exception)
            {
            }

            return ip_to_mac_map;
        }
    }
}
