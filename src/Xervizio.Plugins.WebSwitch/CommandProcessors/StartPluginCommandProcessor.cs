using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.CommandProcessors {
    public class StartPluginCommandProcessor : ICommandProcessor<StartPluginCommand> {
        private ServicePluginHost _host;

        public CommandContext Context { get; set; }

        public StartPluginCommandProcessor(ServicePluginHost host) {
            _host = host;            
        }
        
        public void Process(StartPluginCommand args) {
            _host.StartPlugin(args.PluginName);
            Context.Result = "Plugin [{0}] started".WithTokens(args.PluginName);
        }
    }
}
