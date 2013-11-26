using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Xervizio;

namespace Xervizio.Plugins.WebSwitch {
    using Hosting;

    public class WebSwitchPlugin : HostGateway {
        private WebApiHost _host;

        public WebSwitchPlugin() {}

        public override void Start() {
            ServiceHostContext.Bootstrap(base.HostContext);
            string baseAddress = ConfigurationManager.AppSettings["baseAddress"];
            _host = new WebApiHost(baseAddress);
            _host.Start();
            HostContext.Logger.Info("WebSwitchPlugin started. ({0})", baseAddress);
        }

        public override void Stop() {
            _host.Stop();
            _host.Dispose();
            HostContext.Logger.Info("WebSwitchPlugin stopped.");
        }

        public override ServicePluginState GetState() {
            throw new NotImplementedException();
        }
    }
}
