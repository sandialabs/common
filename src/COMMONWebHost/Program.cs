using COMMONWeb;
using gov.sandia.sld.common.db;
using gov.sandia.sld.common.logging;
using Nancy.Hosting.Self;
using System;
using System.Data.SQLite;

namespace COMMONWebHost
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            string filename = null;
            ILog log = LogManager.GetLogger(typeof(Program));

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-p")
                {
                    if (++i < args.Length)
                        int.TryParse(args[i], out port);
                }
                else if(args[i] == "-f")
                {
                    if (++i < args.Length)
                        filename = args[i];
                }
                else if (args[i] == "-?")
                {
                    ShowUsage();
                    return;
                }
                else
                {
                    Console.WriteLine($"Unknown {args[i]}");
                    ShowUsage();
                    return;
                }
            }

            if (string.IsNullOrEmpty(filename) == false)
            {
                Context.SpecifyFilename(filename);
                Console.WriteLine($"Using database: {filename}");
            }

            Database db = new Database();
            Initializer init = new Initializer(new Initializer.EOptions[] { Initializer.EOptions.SkipSystemCreation });
            init.Initialize(db);

            HostConfiguration config = new HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true },
                UnhandledExceptionCallback = (x) => {
                    log.Error(x);

                    Console.WriteLine(x.Message);
                    Console.WriteLine(x.StackTrace);
                    x = x.InnerException;
                    while(x != null)
                    {
                        Console.WriteLine(x.Message);
                        x = x.InnerException;
                    }
                }
            };

            string url = $"http://localhost:{port}";
            using (var host = new NancyHost(new Uri(url), new COMMONDatabaseBootstrapper(), config))
            {
                host.Start();
                Console.WriteLine($"Running on {url}");
                Console.ReadLine();
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine("COMMONWebHost.exe [-p <port>] [-f <filename>] [-?]");
            Console.WriteLine("  where");
            Console.WriteLine("    <port> is the port the web server will listen on (default 8080)");
            Console.WriteLine("    <filename> is the full path to the COMMON SQLite database");
            Console.WriteLine("    /? shows this help information");
        }
    }
}
