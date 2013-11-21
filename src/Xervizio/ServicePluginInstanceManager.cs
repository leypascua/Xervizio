using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Xervizio {
    public class ServicePluginInstanceManager {        
        private AppDomain _appDomain;
        private IServicePlugin _servicePlugin;

        public ServicePluginInstanceManager(ServicePluginManifest manifest) {
            Manifest = manifest;
        }

        public ServicePluginManifest Manifest { get; private set; }

        public virtual IServicePlugin GetInstance() {
            if (_appDomain != null && _servicePlugin != null) return _servicePlugin;

            UnloadInstance();

            var setup = new AppDomainSetup {
                ApplicationBase = Manifest.PluginBasePath,
                ConfigurationFile = Manifest.PluginConfigurationFile
            };

            _appDomain = AppDomain.CreateDomain(
                Manifest.PluginName, null, setup, new PermissionSet(PermissionState.Unrestricted), null);

            _servicePlugin = (IServicePlugin)_appDomain.CreateInstanceAndUnwrap(Manifest.FullAssemblyName, Manifest.AssemblyEntryPointType);
            return _servicePlugin;
        }

        public virtual void UnloadInstance() {
            if (_servicePlugin != null) {
                var disposable = _servicePlugin as IDisposable;
                if (disposable.Exists()) disposable.Dispose();                
                _servicePlugin = null;
            }
            if (_appDomain != null) {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
        }

    }
}
