using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace gov.sandia.sld.common.db.models
{
    /// <summary>
    /// Information about the ping status of specific IP address. If we know the name of a given device, name will be set to that.
    /// successfulPing is set to 1 if the last ping attempt was successful.
    /// </summary>
    public class NetworkStatus
    {
        /// <summary>
        /// The name of the device pinged, if we know it. Otherwise, it's the IP address.
        /// </summary>
        public string name { get; set; }

        public long deviceID { get; set; }

        /// <summary>
        /// Set to true if the last ping attempt was successful; false otherwise
        /// </summary>
        public bool successfulPing { get; set; }

        /// <summary>
        /// The time/date the last successful ping occurred
        /// </summary>
        public DateTimeOffset? dateSuccessfulPingOccurred { get; set; }

        /// <summary>
        /// The time/date the last ping attempt was made
        /// </summary>
        public DateTimeOffset datePingAttempted { get; set; }

        /// <summary>
        /// The IP address of the ping attempt. May be the same as name if we don't know the name of what was pinged.
        /// </summary>
        public string ipAddress { get; set; }

        /// <summary>
        /// True if it has ever been pinged; false if it has never responded to a ping
        /// </summary>
        public bool hasBeenPinged { get; set; }

        public List<PingAttempt> attempts { get; set; }

        public NetworkStatus()
        {
            deviceID = -1;
            hasBeenPinged = false;
            attempts = new List<PingAttempt>();
        }

        public void AddPingAttempt(PingAttempt attempt)
        {
            attempts.Add(attempt);
        }
    }
}
