namespace gov.sandia.sld.common.utilities
{
    public interface ICollectionTimeRetriever
    {
        /// <summary>
        /// Get the ID of the next collector so we can collect in
        /// priority-order.
        /// </summary>
        /// <returns>null if there isn't one to collect</returns>
        long? GetNextID();
    }
}
