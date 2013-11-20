using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    using Configuration;
    using Utils;

    public class ServicePluginHost {
        private readonly XervizioConfiguration _configuration;        
        private ILogger _logger;
        private HostedPluginFactory _pluginFactory;

        public ServicePluginHost(XervizioConfiguration config, ILogger logger, HostedPluginFactory pluginFactory) {            
            _configuration = config;
            _logger = logger;
            _pluginFactory = pluginFactory;
        }

        public virtual void Start() {
            // locate and enumerate plugins from plugin directory
            IEnumerable<HostedPlugin> plugins = _pluginFactory.GetPlugins().ToList();

            foreach (var plugin in plugins) {
                if (_validPlugins.ContainsKey(plugin.Name)) continue;
                plugin.Load();
                _validPlugins.Add(plugin.Name, plugin);
            }
        }

        public virtual void Shutdown() {
            var keys = _validPlugins.Keys.ToList();
            
            foreach (string key in keys) {
                var plugin = _validPlugins[key];
                plugin.Unload();
                _validPlugins.Remove(key);
                plugin = null;
            }
        }

        private IDictionary<string, HostedPlugin> _validPlugins = new Dictionary<string, HostedPlugin>();
    }
}
