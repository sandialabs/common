using System;
using System.ServiceProcess;

namespace gov.sandia.sld.common
{
    class Program
    {
        static void Main(string[] args)
        {
            // https://haacked.com/archive/2004/06/29/current-directory-for-windows-service-is-not-what-you-expect.aspx/
            System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

#if (!DEBUG)
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new COMMONService() 
            };
            ServiceBase.Run(ServicesToRun);
#else
            COMMONService service = new COMMONService();
            service.Startup();
            bool stop = false;
            while (stop == false)
                global::System.Threading.Thread.Sleep(100);
            service.Shutdown();
#endif
        }
    }
}
