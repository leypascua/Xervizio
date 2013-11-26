using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace Xervizio {
    using Utils;

    public class HostedPluginFactory {        
        private IServicePluginCatalogFactory _catalogFactory;
        private ILogger _logger;
        private string _pluginsPath;
        private Func<ServicePluginHost> _hostFactory;        

        public HostedPluginFactory(Func<ServicePluginHost> hostFactory, IServicePluginCatalogFactory catalogFactory, ILogger logger, string pluginsPath) {
            _hostFactory = hostFactory;
            _catalogFactory = catalogFactory;
            _logger = logger;
            _pluginsPath = pluginsPath;
        }

        public virtual IEnumerable<HostedPlugin> GetPlugins() {
            using (var catalog = _catalogFactory.CreateCatalog(_pluginsPath, _logger)) {
                return catalog
                    .GetPluginManifests()                    
                    .Select(manifest => new HostedPlugin(new ServicePluginInstanceManager(manifest), _hostFactory()));
            }
        }

        public virtual HostedPlugin GetPlugin(string pluginName) {
            return GetPlugins().FirstOrDefault(x => x.PluginManager.Manifest.PluginName == pluginName);
        }
    }
}
