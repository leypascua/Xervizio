using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.Commands {

    public interface PluginCommand {
        string PluginName { get; }
    }

    public class StartPluginCommand : ApplicationCommand, PluginCommand  {
        public string PluginName { get; set; }
    }

    public class StopPluginCommand : ApplicationCommand, PluginCommand {
        public string PluginName { get; set; }
    }
}
