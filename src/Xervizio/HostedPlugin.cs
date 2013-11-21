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

        public HostedPlugin(ServicePluginManifest manifest) {
            Manifest = manifest; /* new ServicePluginManifest {
                PluginName = manifest.PluginName,
                PluginBasePath = manifest.PluginBasePath,
                AssemblyEntryPointType = manifest.AssemblyEntryPointType,
                FullAssemblyName = manifest.FullAssemblyName,
                PluginConfigurationFile = manifest.PluginConfigurationFile
            };*/
        }

        public ServicePluginManifest Manifest { get; private set; }

        public virtual void Load() {
            if (_appDomain != null) return;
            
            var setup = new AppDomainSetup {
                ApplicationBase = Manifest.PluginBasePath,
                ConfigurationFile = Manifest.PluginConfigurationFile
            };

            _appDomain = AppDomain.CreateDomain(
                Manifest.PluginName, null, setup, new PermissionSet(PermissionState.Unrestricted), null);

            _servicePlugin = (IServicePlugin)_appDomain.CreateInstanceAndUnwrap(Manifest.FullAssemblyName, Manifest.AssemblyEntryPointType);
            _servicePlugin.Start();
        }

        public virtual void Restart() {
            Unload();
            Load();
        }

        public virtual void Unload() {
            if (_appDomain == null) return;
            _servicePlugin.Stop();
            _servicePlugin = null;
            AppDomain.Unload(_appDomain);            
            _appDomain = null;
        }

        private AppDomain _appDomain;
        private IServicePlugin _servicePlugin;        
    }
}
