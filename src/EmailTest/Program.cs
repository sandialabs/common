using gov.sandia.sld.common.utilities;
using System;

namespace EmailTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string smtp = string.Empty;
            string from = string.Empty;
            string to = string.Empty;
            string body = string.Empty;

            for(int i = 0; i < args.Length; i++)
            {
                switch(args[i])
                {
                    case "-smtp":
                        if(i + 1 < args.Length)
                            smtp = args[++i];
                        break;
                    case "-from":
                        if(i + 1 < args.Length)
                            from = args[++i];
                        break;
                    case "-to":
                        if(i + 1 < args.Length)
                            to = args[++i];
                        break;
                    case "-body":
                        if(i + 1 < args.Length)
                            body = args[++i];
                        break;
                    default:
                        break;
                }
            }

            if(string.IsNullOrEmpty(smtp) ||
                string.IsNullOrEmpty(from) ||
                string.IsNullOrEmpty(to) ||
                string.IsNullOrEmpty(body))
            {
                ShowUsage();
                return;
            }

            try
            {
                MailSender sender = new MailSender(smtp, from, to);
                MailSender.SentDetails result = sender.SendEmail("Test Email", body);

                Console.WriteLine("Success");
                Console.WriteLine(result);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  emailtest -smtp <SMTP addr> -from <from addr> -to <to addr(s)>, -body <email body>");
        }
    }
}
