using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Xervizio {
    public class ServicePluginInstanceManager {
        private IsolatedInstanceManager _instanceManager;        

        public ServicePluginInstanceManager(ServicePluginManifest manifest) {
            Manifest = manifest;
            _instanceManager = new IsolatedInstanceManager(new IsolatedInstanceManagerSetup {
              ApplicationBase = manifest.PluginBasePath,
              ConfigurationFile = manifest.PluginConfigurationFile,
              FriendlyName = manifest.PluginName,
              FullAssemblyName = manifest.FullAssemblyName,
              AssemblyEntryPointType = manifest.AssemblyEntryPointType,
            });
        }
        
        public ServicePluginManifest Manifest { get; private set; }

        public virtual IServicePlugin GetInstance() {
            return _instanceManager.GetInstance<IServicePlugin>();
        }

        public virtual void UnloadInstance() {
            _instanceManager.UnloadInstance();
        }

    }
}
