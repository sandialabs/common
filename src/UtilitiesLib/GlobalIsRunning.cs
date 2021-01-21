using System.Threading;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Don't really like this class, but sometimes you just need a global boolean
    /// flag that can be used to stop processing if IsRunning is false...
    /// </summary>
    public static class GlobalIsRunning
    {
        public static bool IsRunning { get; private set; }
        public static CancellationTokenSource Source { get; private set; }

        public static void Start()
        {
            IsRunning = true;
            Source = new CancellationTokenSource();
        }

        public static void Stop()
        {
            IsRunning = false;
            Source.Cancel();
        }

        static GlobalIsRunning()
        {
            IsRunning = false;
            Source = new CancellationTokenSource();
        }
    }
}
