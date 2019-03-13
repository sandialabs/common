namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Don't really like this class, but sometimes you just need a global boolean
    /// flag that can be used to stop processing if IsRunning is false...
    /// </summary>
    public static class GlobalIsRunning
    {
        public static bool IsRunning { get; set; }

        static GlobalIsRunning()
        {
            IsRunning = false;
        }
    }
}
