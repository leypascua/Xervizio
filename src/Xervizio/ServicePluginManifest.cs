using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    
    [Serializable]
    public class ServicePluginManifest {
        public string PluginName { get; set; }
        public string PluginBasePath { get; set; }
        public string PluginConfigurationFile { get; set; }
        public string FullAssemblyName { get; set; }
        public string AssemblyEntryPointType { get; set; }
    }
}
