using gov.sandia.sld.common.logging;
using gov.sandia.sld.common.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptDecrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            bool? encrypt = null;
            bool verbose = false;

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i] == "-d")
                    encrypt = false;
                else if (args[i] == "-e")
                    encrypt = true;
                else if (args[i] == "-v")
                    verbose = true;
                else if (encrypt.HasValue)
                    EncryptOrDecrypt(args[i], encrypt.Value, verbose);
            }

            if (encrypt == null)
                ShowUsage();
        }

        static void EncryptOrDecrypt(string str, bool encrypt, bool verbose)
        {
            SimpleEncryptDecrypt sed = new SimpleEncryptDecrypt();

            ConsoleLogger logger = null;
            Action<string> output = null;

            if(verbose)
            {
                logger = new ConsoleLogger(typeof(Program), LogManager.LogLevel.Debug);
                output = (x) => logger.Debug(x);
            }

            string s = null;
            if (encrypt)
                s = sed.Encrypt(str, output);
            else
                s = sed.Decrypt(str, output);

            Console.WriteLine(str + " -> " + s);
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("  EncryptDecrypt [-d|-e] [-v] <text> [<text> *]");
            Console.WriteLine(" where");
            Console.WriteLine("  -d or -e is specified, specifying encryption or decryption");
            Console.WriteLine("  -v optionally outputs verbose information");
            Console.WriteLine(" and");
            Console.WriteLine("  <text> is the text to encrypt/decrypt");
            Console.WriteLine(" <text> can be repeated multiple times");
        }
    }
}
