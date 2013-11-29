using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Http;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.Api {
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
            var processor = CommandRegistry.GetCommandProcesor(new ShutdownHostCommand());
            processor.Execute();
            return true;
        }
    }

    public class HostData {
        public int PluginCount { get; set; }
    };
}
