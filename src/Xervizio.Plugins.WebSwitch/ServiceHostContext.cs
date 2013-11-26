using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xervizio.Plugins.WebSwitch {
    public static class ServiceHostContext {
        public static void Bootstrap(ServicePluginHost hostContext) {
            Current = hostContext;
        }

        public static ServicePluginHost Current { get; private set; }
    }
}
