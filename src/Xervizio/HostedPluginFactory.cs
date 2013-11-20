using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    public class HostedPluginFactory {
        private string _pluginPath;

        public HostedPluginFactory(string pluginPath) {
            _pluginPath = pluginPath;
        }

        public virtual IEnumerable<HostedPlugin> GetPlugins() {
            // ensure that plugin folder exists.

            // foreach folder in the plugins folder...
            //      if folder is valid (an assembly having the folder name as it's name) 
            //      create a HostedPlugin instance.
           
            yield break;
        }
        
    }
}
