using gov.sandia.sld.common.configuration;
using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Diagnostics;

namespace gov.sandia.sld.common.data
{
    public class CollectorID
    {
        public long ID { get; set; }
        public string Name { get; set; }

        public CollectorID(long id, string name)
        {
            ID = id;
            Name = name;
        }

        public CollectorID(CollectorID id)
        {
            ID = id.ID;
            Name = id.Name;
        }
    }

    public class DeviceID
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public DeviceID(long id, string name)
        {
            ID = id;
            Name = name;
        }
    }

    public class DataCollectorContext
    {
        public CollectorID ID { get; private set; }
        public string Name { get { return ID.Name + "." + Type.ToString(); } }
        public ECollectorType Type { get; set; }

        public DataCollectorContext(CollectorID id, ECollectorType collector_type)
        {
            ID = id;
            Type = collector_type;
        }
    }

    /// <summary>
    /// Base class for something that collects data of some sort
    /// </summary>
    public abstract class DataCollector
    {
        public DataCollectorContext Context { get; private set; }

        public DataCollector(DataCollectorContext context)
        {
            Context = context;

            m_lock = new object();
            m_log = LogManager.GetLogger(typeof(DataCollector));
        }

        public virtual void Acquire()
        {
            if (GlobalIsRunning.IsRunning == false)
                return;

            try
            {
                m_log.Debug(Context.Name + ": acquiring");

                Stopwatch watch = Stopwatch.StartNew();
                CollectedData cd = OnAcquire();
                watch.Stop();

                //Debug.Assert(cd != null);

                m_log.Debug(string.Format("{0}: acquisition took {1} ms", Context.Name, watch.ElapsedMilliseconds));

                watch = Stopwatch.StartNew();

                NotifyOfDataAcquired(cd);

                watch.Stop();
                m_log.Debug(string.Format("{0}: notification took {1} ms", Context.Name, watch.ElapsedMilliseconds));
            }
            catch (Exception e)
            {
                m_log.Error(e);
            }
        }

        public abstract CollectedData OnAcquire();

        public void AttachDataAcquiredHandler(DataAcquired a)
        {
            lock (m_lock)
                OnDataAcquired += a;
        }

        public void DetachDataAcquiredHandler(DataAcquired a)
        {
            lock (m_lock)
                OnDataAcquired -= a;
        }

        protected void NotifyOfDataAcquired(CollectedData cd)
        {
            lock (m_lock)
            {
                OnDataAcquired?.Invoke(cd);
            }
        }

        protected object m_lock;
        protected ILog m_log;

        private event DataAcquired OnDataAcquired;
    }
}
