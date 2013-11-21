using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace Xervizio {
    using Utils;

    public class HostedPluginFactory {        
        private ServicePluginCatalogFactory _catalogFactory;
        private ILogger _logger;        

        public HostedPluginFactory(ServicePluginCatalogFactory catalogFactory, ILogger logger) {            
            _catalogFactory = catalogFactory;
            _logger = logger;
        }

        public virtual IEnumerable<HostedPlugin> GetPlugins(string pluginsPath) {
            using (var catalog = _catalogFactory.CreateCatalog(pluginsPath, _logger)) {
                return catalog
                    .GetPluginManifests()
                    .Select(manifest => new HostedPlugin(new ServicePluginInstanceManager(manifest)));
            }
        }
        
    }
}
