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

        public HostedPlugin(string name, string absoluteDirectoryPath) {
            Name = name;
            ApplicationBase = absoluteDirectoryPath;
        }

        public string Name { get; private set; }
        public string ApplicationBase { get; private set; }        

        public virtual void Load() {
            if (_appDomain != null) return;
            
            var setup = new AppDomainSetup {
                ApplicationBase = ApplicationBase,
                ConfigurationFile = BuildConfigurationFilePath(ApplicationBase)
            };

            _appDomain = AppDomain.CreateDomain(
                Name, null, setup, new PermissionSet(PermissionState.Unrestricted), null);

            _servicePlugin = CreatePluginEntryPointInstance(_appDomain, ApplicationBase);
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

        private ServicePlugin CreatePluginEntryPointInstance(AppDomain pluginDomain, string applicationBase) {
            string assemblyName = Path.GetDirectoryName(applicationBase);
            return (ServicePlugin)pluginDomain.CreateInstanceAndUnwrap(assemblyName, PLUGIN_LOADER_TYPENAME);
        }

        private string BuildConfigurationFilePath(string applicationBase) {
            var dllConfig = Path.Combine(applicationBase, Name + ".dll.config");
            if (File.Exists(dllConfig)) return dllConfig;

            var exeConfig = Path.Combine(applicationBase, Name + ".exe.config");
            if (File.Exists(exeConfig)) return exeConfig;

            return string.Empty;
        }

        private AppDomain _appDomain;
        private ServicePlugin _servicePlugin;
    }
}
