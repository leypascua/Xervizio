using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Abstractions;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Xervizio {
    using Utils;
    using Configuration;

    public interface IServicePluginCatalogFactory {
        IServicePluginCatalog CreateCatalog(string pluginsPath, ILogger logger);
    }

    public class ServicePluginCatalogFactory : IServicePluginCatalogFactory {

        public static Func<IServicePluginCatalogFactory> CreateInstance = () => new IsolatedServicePluginCatalogFactory(); // new ServicePluginCatalogFactory(); //

        private ServicePluginCatalogFactory() { }

        public virtual IServicePluginCatalog CreateCatalog(string pluginsPath, ILogger logger) {
            return new ServicePluginCatalog(pluginsPath, new FileSystem(), logger);
        }
    }

    public interface IServicePluginCatalog : IDisposable {
        ServicePluginManifest[] GetPluginManifests();
    }
    
    [Serializable]
    public class ServicePluginCatalog : MarshalByRefObject, IServicePluginCatalog {
        private readonly string _pluginsPath;
        private readonly FileSystem _fileSystem;
        private readonly ILogger _logger;

        public ServicePluginCatalog(string pluginsPath, FileSystem fileSystem, ILogger logger) {
            _pluginsPath = pluginsPath;
            _fileSystem = fileSystem ?? new FileSystem();
            _logger = logger;
        }

        public virtual ServicePluginManifest[] GetPluginManifests() {
            EnsurePluginsPathExist(_pluginsPath);
            IEnumerable<string> possiblePluginDirectories = _fileSystem.Directory.GetDirectories(_pluginsPath);
            var results = new List<ServicePluginManifest>();

            foreach (var candidate in possiblePluginDirectories) {
                string pluginName = _fileSystem.DirectoryInfo.FromDirectoryName(candidate).Name;
                var manifest = NewPluginManifest(candidate, pluginName);
                
                if (manifest.IsNull()) {
                    _logger.Warn("Invalid plugin found: {0}", pluginName);
                    continue;
                }

                results.Add(manifest);
            }

            return results.ToArray();
        }

        public virtual void Dispose() { }

        protected virtual ServicePluginManifest NewPluginManifest(string pluginPath, string pluginName) {            
            
            try {
                string pluginEntryPoint = "{0}.dll".WithTokens(pluginName);
                
                // this will throw a FileNotFoundException when file doesn't exist.
                using (var catalog = new AssemblyCatalog(_fileSystem.Path.Combine(pluginPath, pluginEntryPoint))) {
                    var part = catalog.Parts.First();
                    var export = part.ExportDefinitions.First() as ICompositionElement;

                    var manifest = new ServicePluginManifest {
                        PluginName = pluginName,
                        PluginBasePath = pluginPath,
                        PluginConfigurationFile = "{0}.config".WithTokens(pluginEntryPoint),
                        FullAssemblyName = catalog.Assembly.FullName,
                        AssemblyEntryPointType = export.Origin.DisplayName
                    };

                    return manifest;
                }
            }
            catch {
                return null;
            }
        }

        private void EnsurePluginsPathExist(string pluginsPath) {            
            if (!_fileSystem.Directory.Exists(pluginsPath)) {
                _fileSystem.Directory.CreateDirectory(pluginsPath);
            }
        }
    }
}
