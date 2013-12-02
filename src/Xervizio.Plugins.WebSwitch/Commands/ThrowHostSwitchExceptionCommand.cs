using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.Commands {
    public class ThrowHostSwitchExceptionCommand : ApplicationCommand {
        public Exception OriginalException { get; set; }
    }
}
