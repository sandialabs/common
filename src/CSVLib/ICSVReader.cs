namespace CSVLib
{
    /// <summary>
    /// Interface used to read CSV files. This method in your class that implements
    /// this interface will be repeatedly called for each line in a CSV file.
    /// </summary>
    public interface ICSVReader
    {
        void OnCSVEntry(int row, string[] entries);

        void OnError(string message);
    }
}
