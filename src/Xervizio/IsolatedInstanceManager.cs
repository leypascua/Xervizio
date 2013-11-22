using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Xervizio.Utils;

namespace Xervizio {
    public class IsolatedInstanceManager {

        public static IsolatedInstanceManager CreateInstance<T>(string applicationBase, params object[] constructorArgs) {
            var setup = IsolatedInstanceManagerSetup.Create<T>(
                 () => new IsolatedInstanceManagerSetup {
                    ApplicationBase = applicationBase
                 },
                 constructorArgs
            );

            return new IsolatedInstanceManager(setup);
        }

        public IsolatedInstanceManager(IsolatedInstanceManagerSetup setup) {
            Setup = setup;            
        }

        protected virtual IsolatedInstanceManagerSetup Setup { get; private set; }
        
        public virtual T GetInstance<T>() where T : class {
            if (InstanceExists()) return (T)_instance;

            UnloadInstance();

            var appDomainSetup = new AppDomainSetup {
                ApplicationBase = Setup.ApplicationBase,
                ConfigurationFile = Setup.ConfigurationFile
            };

            _appDomain = AppDomain.CreateDomain(
                Setup.FriendlyName, null, appDomainSetup, new PermissionSet(PermissionState.Unrestricted), null);

            _instance = CreateInstanceAndUnwrap<T>(_appDomain, Setup);

            return (T)_instance;
        }

        public virtual void UnloadInstance() {
            if (!InstanceExists()) return;

            if (_instance != null) {
                var disposable = _instance as IDisposable;
                if (disposable.Exists()) {
                    disposable.Dispose();
                    disposable = null;
                }

                _instance = null;
            }

            if (_appDomain.Exists()) {
                AppDomain.Unload(_appDomain);
                _appDomain = null;
            }
        }

        private object CreateInstanceAndUnwrap<T>(AppDomain appDomain, IsolatedInstanceManagerSetup setup) where T : class {
            var result = appDomain.CreateInstanceAndUnwrap(setup.FullAssemblyName, setup.AssemblyEntryPointType,
                true, BindingFlags.CreateInstance, null, setup.ConstructorParameters,
                Thread.CurrentThread.CurrentCulture, new object[0]);

            return result;
        }

        private bool InstanceExists() {
            return _appDomain.Exists() && _instance.Exists();
        }

        private AppDomain _appDomain;
        private object _instance;
    }

    public class IsolatedInstanceManagerSetup {        
        const string CODEPREFIX = "IsolatedInstanceManager";
        const string REFERENCE = "1234567890hArLeYdOlOrpAsCuA";
        static readonly IQueryable<string> NO_BLACKLIST = Enumerable.Empty<string>().AsQueryable();

        public static IsolatedInstanceManagerSetup Create<T>(Func<IsolatedInstanceManagerSetup> factory, params object[] constructorArgs) {
            var result = factory();
            result.FullAssemblyName = typeof(T).Assembly.FullName;
            result.AssemblyEntryPointType = typeof(T).FullName;
            result.ConstructorParameters = constructorArgs;

            return result;
        }

        public IsolatedInstanceManagerSetup(string fullAssemblyName = null, string assemblyEntryPointType = null, string friendlyName = null) {
            ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            if (friendlyName.IsNullOrEmpty()) {
                string unique = DateTime.UtcNow.ToString("yyyyMMddThhmmssfff");
                FriendlyName = "IsolatedInstanceManager_{0}".WithTokens(unique);
            }

            FullAssemblyName = fullAssemblyName;
            AssemblyEntryPointType = assemblyEntryPointType;
            ConstructorParameters = new object[0];
        }
        
        public string ApplicationBase { get; set; }
        public string ConfigurationFile { get; set; }
        public string FriendlyName { get; set; }
        public string FullAssemblyName { get; set; }
        public string AssemblyEntryPointType { get; set; }
        public object[] ConstructorParameters { get; set; }
    }
}
