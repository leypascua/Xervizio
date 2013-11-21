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
                    s.ConstructUsing(name => CreatePluginHost(config, logger));
                    s.WhenStarted(svc => svc.Start());
                    s.WhenStopped(svc => svc.Shutdown());
                });

                host.SetServiceName(config.ServiceName);
                host.SetDisplayName(config.DisplayName);
                host.SetDescription(config.ServiceDescription);
            });
        }

        static ServicePluginHost CreatePluginHost(XervizioConfiguration config, ILogger logger) {
            var servicePluginCatalogFactory = ServicePluginCatalogFactory.CreateInstance();
            var pluginFactory = new HostedPluginFactory(servicePluginCatalogFactory, logger);
            return new ServicePluginHost(config, logger, pluginFactory);
        }
    }
}
