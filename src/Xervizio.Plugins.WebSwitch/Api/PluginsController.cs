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
        public virtual HttpResponseMessage Stop(PluginCommand cmd) {
            return HandleCommand(() => _pluginHost.StopPlugin(cmd.PluginName));
        }

        [HttpPost]
        public virtual HttpResponseMessage Start(PluginCommand cmd) {
            return HandleCommand(() => _pluginHost.StartPlugin(cmd.PluginName));
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

    public class PluginCommand {
        public string PluginName { get; set; }
    }
}
