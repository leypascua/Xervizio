using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Reflection;

namespace Xervizio {
    public class HostedPlugin {

        const string PLUGIN_LOADER_TYPENAME = "Xervizio.Plugins.ServicePluginLoader";

        public HostedPlugin(ServicePluginInstanceManager pluginManager, ServicePluginHost host) {
            PluginManager = pluginManager;
            Host = host;
        }

        public ServicePluginInstanceManager PluginManager { get; private set; }
        public ServicePluginHost Host { get; private set; }

        public virtual void Load() {
            try {
                var serviceInstance = PluginManager.GetInstance();                

                var hostGateway = serviceInstance as HostGateway;
                if (hostGateway.Exists()) {
                    hostGateway.HostContext = Host;
                }

                serviceInstance.Start();
            }
            catch (Exception ex) {
                Unload();
                throw new ServicePluginLoadingException(ex);
            }
        }

        public virtual void Restart() {
            Unload();
            Load();
        }

        public virtual void Unload() {
            PluginManager.UnloadInstance();
        }
    }
}
