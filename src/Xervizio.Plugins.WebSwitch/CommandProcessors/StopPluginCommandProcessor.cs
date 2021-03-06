﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.CommandProcessors {
    public class StopPluginCommandProcessor : ICommandProcessor<StopPluginCommand> {
        private ServicePluginHost _host;

        public CommandContext Context { get; set; }

        public StopPluginCommandProcessor(ServicePluginHost host) {
            _host = host;
        }

        public void Process(StopPluginCommand args) {
            _host.StopPlugin(args.PluginName);
            Context.Result = "Plugin [{0}] stopped.".WithTokens(args.PluginName);
        }
    }
}
