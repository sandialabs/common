using System;

namespace gov.sandia.sld.common.logging
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(Type type, LogManager.LogLevel level = LogManager.LogLevel.Error)
            : base(type, level)
        {
        }

        protected override void DoOutput(string output)
        {
            Console.WriteLine(output);
        }
    }
}
