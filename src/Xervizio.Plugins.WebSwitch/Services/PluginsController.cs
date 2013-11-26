using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
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

    }
}
