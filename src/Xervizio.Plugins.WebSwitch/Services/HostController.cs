using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace Xervizio.Plugins.WebSwitch.Services {
    public class HostController : ApiController {
        private ServicePluginHost _host;

        public HostController() : this(null) { }

        public HostController(ServicePluginHost host) {
            _host = host ?? ServiceHostContext.Current;
        }
                
        [ActionName("DefaultAction")]
        public HostData Get() {
            return new HostData {
                PluginCount = _host.GetPlugins().Length
            };
        }

        [HttpPost]
        [ActionName("Shutdown")]
        public bool Shutdown() {            
            _host.Shutdown();
            return true;
        }
    }

    public class HostData {
        public int PluginCount { get; set; }
    };
}
