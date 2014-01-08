using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Xervizio.Configuration;
using Xervizio.Utils;

namespace Xervizio.Host {
    public class SimpleStandaloneHost {
        public static void Start(XervizioConfiguration config, ILogger logger) {
            if (!config.IsOnline) {
                logger.Info("Service Plugin Host is Offline. Stopping Service.");
                return;
            }
            
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
            var pluginFactory = new HostedPluginFactory(() => ServicePluginHostContext.Current, servicePluginCatalogFactory, logger, config.GetPluginsPath());
            Action shutdownHost = () => Environment.Exit(0);
            return ServicePluginHostContext.Bootstrap(config, logger, pluginFactory, shutdownHost);
        }
    }
}
