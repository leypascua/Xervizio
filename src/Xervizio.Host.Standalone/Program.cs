using System;

namespace Xervizio.Host.Standalone {
    using Configuration;
    using Utils;

    class Program {

        static void Main(string[] args) {
            Banner();

            XervizioConfiguration config = XervizioConfigurationContext.Current.Settings;
            ILogger logger = new SimpleLogger();

            SimpleStandaloneHost.Start(config, logger);
        }

        static void Banner() {
            Console.WriteLine();
            Console.WriteLine("Xervizio Stand-alone Service Host");
            Console.WriteLine("Originally developed by leypascua. Contributions are welcome.");
            Console.WriteLine();
        }
    }
}
