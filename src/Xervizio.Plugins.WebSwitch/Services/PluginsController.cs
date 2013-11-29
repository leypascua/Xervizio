using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Xervizio.Data;

namespace Xervizio.Plugins.WebSwitch.Services {
        
    public class PluginsController : ApiController {
        private ServicePluginHost _pluginHost;

        public PluginsController() : this(null) {}

        public PluginsController(ServicePluginHost host) {
            _pluginHost = host ?? ServiceHostContext.Current;
        }
                
        public virtual IEnumerable<PluginData> Get() {
            var results = _pluginHost.GetPlugins();
            return results;
        }

        public virtual PluginData Get(string id) {
            return _pluginHost.GetPlugins()
                .FirstOrDefault(x => x.Name == id);
        }

        [HttpPost]        
        public virtual bool Stop(PluginCommand cmd) {            
            try {
                _pluginHost.StopPlugin(cmd.PluginName);
            }
            catch {
                return false; 
            }

            return true;
        }

        [HttpPost]
        public virtual bool Start(PluginCommand cmd) {
            try {
                _pluginHost.StartPlugin(cmd.PluginName);
            }
            catch {
                return false;
            }

            return true;
        }
    }

    public class PluginCommand {
        public string PluginName { get; set; }
    }
}
