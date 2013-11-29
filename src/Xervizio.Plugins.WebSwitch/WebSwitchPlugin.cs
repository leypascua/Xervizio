using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Xervizio;

namespace Xervizio.Plugins.WebSwitch {
    using Hosting;
    using Infrastructure;
    using Xervizio.Plugins.WebSwitch.CommandProcessors;

    public class WebSwitchPlugin : HostGateway {
        private WebApiHost _host;

        public WebSwitchPlugin() {}

        public override void Start() {
            ServiceHostContext.Bootstrap(base.HostContext);
            RegisterCommands(base.HostContext);
            StartApiServer();
        }

        private static void RegisterCommands(ServicePluginHost host) {
            CommandRegistry.Register(() => new ShutdownHostCommandProcessor(host));
            CommandRegistry.Register(() => new StartPluginCommandProcessor(host));
            CommandRegistry.Register(() => new StopPluginCommandProcessor(host));
        }

        private void StartApiServer() {
            
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
