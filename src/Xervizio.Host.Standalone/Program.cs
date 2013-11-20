using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Topshelf.HostConfigurators;
using Topshelf.ServiceConfigurators;

namespace Xervizio.Host.Standalone {
    using Configuration;
    using Utils;

    class Program {

        static void Main(string[] args) {
            XervizioConfiguration config = XervizioConfigurationContext.Current.Settings;
            ILogger logger = new SimpleLogger();
            
            HostFactory.Run(host => {
                host.Service<ServicePluginHost>(s => {
                    s.ConstructUsing(name => new ServicePluginHost(config, logger, new HostedPluginFactory(config.PluginLocation)));
                    s.WhenStarted(svc => svc.Start());
                    s.WhenStopped(svc => svc.Shutdown());
                });

                host.SetServiceName(config.ServiceName);
                host.SetDisplayName(config.DisplayName);
                host.SetDescription(config.ServiceDescription);
            });
        }
    }
}
