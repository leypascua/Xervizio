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
            var plugins = _pluginFactory.GetPlugins(_configuration.GetPluginsPath()).ToList();

            _logger.Info("Found {0} plugin(s).", plugins.Count);

            foreach (var plugin in plugins) {
                if (_validPlugins.ContainsKey(plugin.PluginManager.Manifest.PluginName)) continue;
                _logger.Info("Now loading {0}", plugin);

                try {
                    plugin.Load();
                    _validPlugins.Add(plugin.PluginManager.Manifest.PluginName, plugin);
                }
                catch (ServicePluginLoadingException ex) {
                    _logger.Error(ex);
                }
            }
        }

        public virtual void Shutdown() {
            var keys = _validPlugins.Keys.ToList();
            
            foreach (string key in keys) {
                var plugin = _validPlugins[key];
                plugin.Unload();
                _validPlugins.Remove(key);
                plugin = null;
                _logger.Info("Unloaded plugin: {0}", key);
            }
        }

        private IDictionary<string, HostedPlugin> _validPlugins = new Dictionary<string, HostedPlugin>();
    }
}
