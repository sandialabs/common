using gov.sandia.sld.common.utilities;
using System.Collections.Generic;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// Used to get the collector IDs for what should be collected next. This uses the
    /// DB to get the IDs.
    /// </summary>
    public class DBCollectionTimeRetriever : ICollectionTimeRetriever
    {
        /// <summary>
        /// Returns the full list of IDs that need to be collected.
        /// </summary>
        public List<long> AllIDs { get { return new List<long>(m_collector_ids); } }

        public DBCollectionTimeRetriever(SQLiteConnection conn)
        {
            CollectionTime d = new CollectionTime(conn);
            m_collector_ids = new Queue<long>(d.GetCollectorIDs());
        }

        public long? GetNextID()
        {
            long? id = null;
            if (m_collector_ids.Count > 0)
                id = m_collector_ids.Dequeue();
            return id;
        }

        private Queue<long> m_collector_ids;
    }
}
