using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    using Configuration;
    using Utils;
    using Data;
    using System.Threading;

    public interface ServicePluginHost {
        ILogger Logger { get; }
        
        PluginData[] GetPlugins();
        bool KeepAlive();
        void Shutdown();
        void Start();
        void StartPlugin(string pluginName);
        void StopPlugin(string pluginName);
    }

    public sealed class ServicePluginHostContext : MarshalByRefObject, ServicePluginHost {
        private readonly XervizioConfiguration _configuration;        
        private readonly ILogger _logger;
        private readonly HostedPluginFactory _pluginFactory;
        private static readonly object _syncLock = new object();

        public static ServicePluginHost Current { get; private set; }

        public static ServicePluginHost Bootstrap(XervizioConfiguration config, ILogger logger, HostedPluginFactory pluginFactory, Action shutdownHost) {
            if (Current.IsNull()) {
                lock (_syncLock) {
                    if (Current.IsNull()) {
                        Current = new ServicePluginHostContext(config, logger, pluginFactory, shutdownHost);
                    }
                }
            }

            return Current;
        }

        private ServicePluginHostContext(XervizioConfiguration config, ILogger logger, HostedPluginFactory pluginFactory, Action shutdownHost) {            
            _configuration = config;
            _logger = logger;
            _pluginFactory = pluginFactory;
            _shutdownHost = shutdownHost;
        }

        ILogger ServicePluginHost.Logger {
            get { return _logger; }
        }

        bool ServicePluginHost.KeepAlive() {
            return true;
        }

        void ServicePluginHost.Start() {
            // locate and enumerate plugins from plugin directory
            var plugins = _pluginFactory.GetPlugins().ToList();

            _logger.Info("[********** Starting Service Plugin Host **********]");
            _logger.Info("Found {0} plugin(s).", plugins.Count);

            foreach (var plugin in plugins) {
                if (_validPlugins.ContainsKey(plugin.PluginManager.Manifest.PluginName)) continue;                
                StartPlugin(plugin);
            }
        }

        PluginData[] ServicePluginHost.GetPlugins() {
            var results = new List<PluginData>();
            var plugins = _pluginFactory.GetPlugins();

            foreach (var item in plugins) {
                results.Add(new PluginData {
                    Name = item.PluginManager.Manifest.PluginName,
                    IsRunning = _validPlugins.ContainsKey(item.PluginManager.Manifest.PluginName)
                });
            }

            return results.ToArray();
        }

        void ServicePluginHost.StartPlugin(string pluginName) {
            HostedPlugin plugin = _pluginFactory.GetPlugin(pluginName);
            Protect.Against<ServicePluginLoadingException>(plugin.IsNull(), 
                "Plugin {0} cannot be found or does not exist.", pluginName);

            StartPlugin(plugin, true);
        }

        void ServicePluginHost.StopPlugin(string pluginName) {
            Protect.AgainstInvalidOperation(!_validPlugins.ContainsKey(pluginName), "Plugin {0} is not loaded.", pluginName);
            var plugin = _validPlugins[pluginName];
            StopPlugin(plugin);
        }

        void ServicePluginHost.Shutdown() {
            Action performShutdown = () => {                
                _logger.Warn("Commencing shutdown in 5 seconds");
                Thread.Sleep(5000);
                ShutdownCompletely();
            };

            performShutdown.BeginInvoke(new AsyncCallback(ar => {}), null);
        }

        private void ShutdownCompletely() {
            var keys = _validPlugins.Keys.ToList();

            foreach (string key in keys) {
                var plugin = _validPlugins[key];
                StopPlugin(plugin);
            }

            _logger.Info("[********** Shutting Down Service Plugin Host **********]");
            _shutdownHost();
        }

        private void StartPlugin(HostedPlugin plugin, bool throwOnError = false) {
            string pluginName = plugin.PluginManager.Manifest.PluginName;
            _logger.Info("Now loading {0}", pluginName);

            if (_validPlugins.ContainsKey(pluginName)) {
                _logger.Info("Plugin {0} is already loaded.", pluginName);
                return;
            }

            try {
                plugin.Load();
                _validPlugins.Add(pluginName, plugin);
            }
            catch (ServicePluginLoadingException ex) {
                _logger.Error(ex);

                if (throwOnError) {
                    throw;
                }
            }
        }

        private void StopPlugin(HostedPlugin plugin) {
            try {
                plugin.Unload();
            }
            finally {
                _validPlugins.Remove(plugin.PluginManager.Manifest.PluginName);
                _logger.Info("Unloaded plugin: {0}", plugin.PluginManager.Manifest.PluginName);
            }
        }

        private IDictionary<string, HostedPlugin> _validPlugins = new Dictionary<string, HostedPlugin>();
        private Action _shutdownHost;
    }
}
