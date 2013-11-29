using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Xervizio.Data;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.Api {
        
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
        public virtual HttpResponseMessage Stop(StopPluginCommand cmd) {
            var command = CommandRegistry.GetCommandProcesor(cmd);
            return HandleCommand(() => command.Execute());
        }

        [HttpPost]
        public virtual HttpResponseMessage Start(StartPluginCommand cmd) {
            var command = CommandRegistry.GetCommandProcesor(cmd);
            return HandleCommand(() => command.Execute());
        }

        private HttpResponseMessage HandleCommand(Action callback) {
            try {
                callback();
            }
            catch (Exception ex) {
                var msg = new HttpResponseMessage(HttpStatusCode.BadRequest);
                msg.ReasonPhrase = ex.Message;
                return msg;
            }

            return new HttpResponseMessage();
        }
    }
}
