using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Xervizio {
    using Utils;

    public class IsolatedServicePluginCatalogFactory : ServicePluginCatalogFactory {

        public override IServicePluginCatalog CreateCatalog(string pluginsPath, ILogger logger) {
            if (_pluginCatalog != null) return _pluginCatalog;

            _hostDomain = AppDomain.CreateDomain(this.GetType().FullName);
            _pluginCatalog = (IServicePluginCatalog)_hostDomain.CreateInstanceAndUnwrap(this.GetType().Assembly.FullName, typeof(IsolatedServicePluginCatalog).FullName,
                true, BindingFlags.CreateInstance, null, new object[] { pluginsPath, logger, this },
                Thread.CurrentThread.CurrentCulture, new object[0]);

            return _pluginCatalog;
        }

        public virtual void ReleaseInstance() {
            if (_hostDomain == null || _pluginCatalog == null) return;

            _pluginCatalog = null;
            AppDomain.Unload(_hostDomain);
            _hostDomain = null;
        }

        private IServicePluginCatalog _pluginCatalog;
        private AppDomain _hostDomain;
    }
    
    
    public class IsolatedServicePluginCatalog : MarshalByRefObject, IServicePluginCatalog {

        public IsolatedServicePluginCatalog(string pluginsPath, ILogger logger, IsolatedServicePluginCatalogFactory catalogFactory) {            
            CatalogFactory = catalogFactory;
            Catalog = new ServicePluginCatalog(pluginsPath, new FileSystem(), logger);
        }
        
        protected virtual IServicePluginCatalog Catalog { get; private set; }
        protected virtual IsolatedServicePluginCatalogFactory CatalogFactory { get; private set; }

        public IEnumerable<ServicePluginManifest> GetPluginManifests() {
            return Catalog.GetPluginManifests();
        }

        #region IDisposable

        public void Dispose() {
            Dispose(true);
        }

        private void Dispose(bool isDisposing) {
            if (isDisposing && !_isDisposed) {
                Catalog.Dispose();
                Catalog = null;

                CatalogFactory.ReleaseInstance();

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
