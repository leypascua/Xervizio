using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.CommandProcessors {
    public class ShutdownHostCommandProcessor : ICommandProcessor<ShutdownHostCommand> {
        private ServicePluginHost _host;

        public CommandContext Context { get; set; }

        public ShutdownHostCommandProcessor(ServicePluginHost host) {
            _host = host;
        }
        
        public void Process(ShutdownHostCommand args) {
            _host.Shutdown();
            Context.Result = "Xervizio Service Host is Shutting Down...";
        }
    }
}
