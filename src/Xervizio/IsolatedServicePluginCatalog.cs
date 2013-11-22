using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Xervizio {
    using Utils;
        
    public class IsolatedServicePluginCatalogFactory : IServicePluginCatalogFactory {

        public virtual IServicePluginCatalog CreateCatalog(string pluginsPath, ILogger logger) {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            return new IsolatedServicePluginCatalog(basePath, pluginsPath, logger);
        }
    }
    
    public class IsolatedServicePluginCatalog : IServicePluginCatalog {

        private string _pluginsPath;
        private ILogger _logger;
        private IsolatedInstanceManager _instanceManager;

        public IsolatedServicePluginCatalog(string basePath, string pluginsPath, ILogger logger) {
            _pluginsPath = pluginsPath;
            _logger = logger;

            _instanceManager = IsolatedInstanceManager.CreateInstance<ServicePluginCatalog>(
                basePath, pluginsPath, null, logger);
        }

        public ServicePluginManifest[] GetPluginManifests() {
            var catalog = _instanceManager.GetInstance<IServicePluginCatalog>();
            return catalog.GetPluginManifests();
        }

        #region IDisposable

        public void Dispose() {
            Dispose(true);
        }

        private void Dispose(bool isDisposing) {
            if (isDisposing && !_isDisposed) {
                _instanceManager.UnloadInstance();
                _instanceManager = null;
                System.GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }

        ~IsolatedServicePluginCatalog() {
            Dispose(!_isDisposed);
        }

        private bool _isDisposed;

        #endregion
    }
}
