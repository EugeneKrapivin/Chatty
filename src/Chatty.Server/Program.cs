using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans.Runtime.Configuration;

namespace Chatty.Server
{
    public class Program
    {
        private static OrleansHostWrapper _hostWrapper;

        public static int Main(string[] args)
        {
            var exitCode = StartSilo(args);

            Console.WriteLine("Press Enter to terminate...");
            Console.ReadLine();

            exitCode += ShutdownSilo();

            //either StartSilo or ShutdownSilo failed would result on a non-zero exit code. 
            return exitCode;
        }

        private static int StartSilo(string[] args)
        {
            // define the cluster configuration
            var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.AddMemoryStorageProvider();

            // config.Defaults.DefaultTraceLevel = Orleans.Runtime.Severity.Verbose3;

            _hostWrapper = new OrleansHostWrapper(config, args);
            return _hostWrapper.Run();
        }

        private static int ShutdownSilo()
        {
            return _hostWrapper?.Stop() ?? 0;
        }
    }
}
