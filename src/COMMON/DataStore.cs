using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.data;
using gov.sandia.sld.common.data.database;
using gov.sandia.sld.common.data.wmi;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.db.models;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System.Collections.Generic;

namespace gov.sandia.sld.common
{
    /// <summary>
    /// A collection of data collectors. Tell the DataStore to Acquire()
    /// and it will go through all the collectors and tell each one
    /// to acquire its data
    /// </summary>
    public abstract class BaseDevice
    {
        /// <summary>
        /// The name of the device that we are collecting data from
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of all the DataCollectors the device uses
        /// </summary>
        public List<DataCollector> Collectors { get; private set; }

        protected DataStorage Storage { get; private set; }

        public BaseDevice(string name, DataStorage storage)
        {
            Name = name;
            Collectors = new List<DataCollector>();
            Storage = storage;
        }

        //public virtual List<DataCollector> GetCollectors(ICollectionTimeRetriever tr)
        //{
        //    List<DataCollector> collectors = new List<DataCollector>();

        //    // We want to get the collectors in the same order presented to us by the
        //    // retriever because they're in priority order.
        //    int? id = tr.GetNextID();
        //    while(id != null)
        //    {
        //        DataCollector dc = Collectors.Find(c => c.Context.ID.ID == id.Value);
        //        if (dc != null)
        //            collectors.Add(dc);
        //        id = tr.GetNextID();
        //    }
        //    return collectors;
        //}

        /// <summary>
        /// Called by the child classes to add a DataCollector to the
        /// set of collectors, and hook in the OnDataCollected
        /// method to handle the data that's been collected.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        protected DataCollector Collect(DataCollector d)
        {
            Collectors.Add(d);
            d.AttachDataAcquiredHandler(OnDataCollected);
            return d;
        }

        /// <summary>
        /// Called when data has been collected. Right now it just
        /// tells the database to store it, but in the past, for debugging,
        /// we'd write out the JSON representation of the data.
        /// </summary>
        /// <param name="d"></param>
        protected void OnDataCollected(CollectedData d)
        {
            //foreach(Data data in d)
            //    Console.WriteLine(Name + ": " + JsonConvert.SerializeObject(data));
            Storage.SaveData(d);
        }
    }

    /// <summary>
    /// A DataStore specifically for the System
    /// </summary>
    public class SystemDevice : BaseDevice
    {
        public List<Device> Devices { get; set; }

        public SystemDevice(SystemConfiguration config, DataStorage storage)
            : base("System", storage)
        {
            m_log = LogManager.GetLogger(typeof(SystemDevice));

            Devices = new List<Device>();

            config.devices.ForEach(m => {if(m.type != EDeviceType.System) Devices.Add(new Device(m, this, Storage));});

            DeviceInfo system = config.devices.Find(d => d.type == EDeviceType.System);
            CollectorInfo ping = null;
            if (system != null)
                ping = system.collectors.Find(c => c.collectorType == ECollectorType.Ping);
            
            if(ping != null && ping.isEnabled)
                Collect(new PingCollector(new CollectorID(ping.id, Name)));
        }

        public List<DataCollector> GetCollectors(ICollectionTimeRetriever tr)
        {
            List<DataCollector> collectors = new List<DataCollector>();

            // We want to get the collectors in the same order presented to us by the
            // retriever because they're in priority order.
            long? id = tr.GetNextID();
            while (id != null)
            {
                // First, check our own collectors to see if this collector is one of ours
                DataCollector dc = Collectors.Find(c => c.Context.ID.ID == id);
                if (dc == null)
                {
                    // Not one of ours, so see if it belongs to one of our children
                    foreach(Device dev in Devices)
                    {
                        dc = dev.Collectors.Find(c => c.Context.ID.ID == id);
                        if (dc != null)
                            break;
                    }
                }

                if (dc != null)
                    collectors.Add(dc);
                else
                    m_log.ErrorFormat("GetCollectors: unable to find collector {0}", id);

                id = tr.GetNextID();
            }

            return collectors;
        }

        private ILog m_log;
    }

    /// <summary>
    /// A DataStore for the other collectors
    /// </summary>
    public class Device : BaseDevice
    {
        public Device(DeviceInfo device, BaseDevice d, DataStorage storage)
            : base(device.name, storage)
        {
            m_device_info = device;
            CreateDataCollectors();
        }

        private void CreateDataCollectors()
        {
            if (m_device_info.type.IsWindowsMachine())
            {
                Remote ri = new Remote(m_device_info.ipAddress, m_device_info.username, m_device_info.password);

                CollectorInfo ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.Memory);
                if (ci != null && ci.isEnabled)
                    Collect(new MemoryUsageCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.Disk);
                if (ci != null && ci.isEnabled)
                    Collect(new DiskUsageCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.CPUUsage);
                if (ci != null && ci.isEnabled)
                    Collect(new CPUUsageCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.NICUsage);
                if (ci != null && ci.isEnabled)
                    Collect(new NICUsageCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.Uptime);
                if (ci != null && ci.isEnabled)
                    Collect(new UptimeCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.LastBootTime);
                if (ci != null && ci.isEnabled)
                    Collect(new LastBootTimeCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.Processes);
                if (ci != null && ci.isEnabled)
                    Collect(new ProcessesCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.InstalledApplications);
                if (ci != null && ci.isEnabled)
                    Collect(new ApplicationsCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.Services);
                if (ci != null && ci.isEnabled)
                    Collect(new ServicesCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.SystemErrors);
                if (ci != null && ci.isEnabled)
                    Collect(new SystemErrorLogCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.ApplicationErrors);
                if (ci != null && ci.isEnabled)
                    Collect(new ApplicationErrorLogCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.UPS);
                if (ci != null && ci.isEnabled)
                    Collect(new UPSCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.DatabaseSize);
                if (ci != null && ci.isEnabled)
                    Collect(new DatabaseSizeCollector(new CollectorID(ci.id, Name), m_device_info.type == EDeviceType.Server, new DatabaseCollectorFactory()));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.DiskSpeed);
                if (ci != null && ci.isEnabled)
                    Collect(new DiskSpeedCollector(new CollectorID(ci.id, Name), ri));

                ci = m_device_info.collectors.Find(c => c.collectorType == ECollectorType.SMART);
                if (ci != null && ci.isEnabled)
                    Collect(new SMARTCollector(new CollectorID(ci.id, Name), ri));

                //ci = m_device_info.collectors.Find(c => c.collectorType == CollectorType.AntiVirus);
                //if (ci != null && ci.isEnabled)
                //    Collect(new AntiVirusCollector(new CollectorID(ci.id, Name), ri));

                //ci = m_device_info.collectors.Find(c => c.collectorType == CollectorType.Firewall);
                //if (ci != null && ci.isEnabled)
                //    Collect(new FirewallCollector(new CollectorID(ci.id, Name), ri));

            }
        }

        private DeviceInfo m_device_info;
    }
}
