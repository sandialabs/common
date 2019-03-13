using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace gov.sandia.sld.common.db.models
{
    public class CollectorInfo
    {
        [JsonIgnore]
        public CollectorID CID { get; set; }

        public long id { get { return CID.ID; } set { CID.ID = value; } }

        [JsonIgnore]
        /// <summary>
        /// The ID of the device this collector belongs to
        /// </summary>
        public DeviceID DID { get; set; }

        public long deviceID { get { return DID.ID; } }

        /// <summary>
        /// The name of the collector (i.e. "System.Ping", or "Server.Disk")
        /// </summary>
        public string name { get { return CID.Name; } set { CID.Name = value; } }

        /// <summary>
        /// The type of collector (ping, memory, CPU usage, etc.)
        /// See the CollectorType enum.
        /// </summary>
        public ECollectorType collectorType { get; set; }

        [JsonIgnore]
        public DataCollectorContext DCContext { get { return new DataCollectorContext(CID, collectorType); } }

        /// <summary>
        /// Used to enable/disable the collector
        /// </summary>
        public bool isEnabled { get; set; }

        /// <summary>
        /// How frequently, in minutes, the data should be collected
        /// </summary>
        public int frequencyInMinutes { get; set; }

        [JsonIgnore]
        public TimeSpan Frequency { get { return TimeSpan.FromMinutes(frequencyInMinutes); } }

        /// <summary>
        /// Certain collectors aren't configurable, so we can skip them
        /// </summary>
        /// <returns>true if the collector can be skipped; false if it should be displayed</returns>
        [JsonIgnore]
        public bool SkipConfiguration { get; private set; }

        public DateTimeOffset? lastCollectionAttempt { get; set; }
        public DateTimeOffset? lastCollectedAt { get; set; }
        public DateTimeOffset? nextCollectionTime { get; set; }
        public bool successfullyCollected { get; set; }
        public bool isBeingCollected { get; set; }

        //private CollectorInfo()
        //{
        //    CID = new CollectorID(-1, string.Empty);
        //    DID = new DeviceID(-1, string.Empty);
        //    collectorType = ECollectorType.Unknown;
        //    isEnabled = true;
        //    frequencyInMinutes = collectorType.GetFrequencyInMinutes();
        //    SkipConfiguration = collectorType.GetSkipConfiguration();
        //    lastCollectionAttempt = lastCollectedAt = nextCollectionTime = null;
        //    successfullyCollected = true;
        //    isBeingCollected = false;
        //}

        public CollectorInfo(ECollectorType type)
        {
            CID = new CollectorID(-1, string.Empty);
            DID = new DeviceID(-1, string.Empty);
            collectorType = type;
            isEnabled = true;
            frequencyInMinutes = collectorType.GetFrequencyInMinutes();
            SkipConfiguration = collectorType.GetSkipConfiguration();
            lastCollectionAttempt = lastCollectedAt = nextCollectionTime = null;
            successfullyCollected = true;
            isBeingCollected = false;
        }

        public static List<CollectorInfo> FromCollectorTypes(List<ECollectorType> types)
        {
            List<CollectorInfo> infos = new List<CollectorInfo>();
            types.ForEach(t => infos.Add(new CollectorInfo(t)));
            return infos;
        }

        public override string ToString()
        {
            return string.Format("CollectorInfo: id {0}, name {1}, collectorType {2}, isEnabled {3}, frequencyInMinutes {4}",
                id, name, collectorType, isEnabled, frequencyInMinutes);
        }

        public void Merge(CollectorInfo other)
        {
            frequencyInMinutes = other.frequencyInMinutes;
            isEnabled = other.isEnabled;
        }
    }
}
