using System;
using System.Data.SQLite;

namespace gov.sandia.sld.common.db
{
    /// <summary>
    /// Class used to ensure that when a collector is collecting data, the DB
    /// is updated to reflect that, and also to ensure that once the collection
    /// has completed, the DB is updated to reflect that as well.
    /// Uses IDisposable, so wrap this in a using(){} block and if any
    /// exceptions occur Dispose() will be called and the DB will be updated.
    /// </summary>
    public class BeingCollected : IDisposable
    {
        public long CollectorID { get; private set; }
        public SQLiteConnection Conn { get; private set; }

        public BeingCollected(long collector_id, SQLiteConnection conn)
        {
            CollectorID = collector_id;
            Conn = conn;

            CollectionTime ct = new CollectionTime(Conn);
            ct.UpdateCollectionAttemptTime(collector_id, DateTimeOffset.Now);
            ct.UpdateNextCollectionTime(collector_id);
            ct.BeingCollected(collector_id, true);
        }

        public void Dispose()
        {
            CollectionTime ct = new CollectionTime(Conn);
            ct.BeingCollected(CollectorID, false);
        }
    }
}
