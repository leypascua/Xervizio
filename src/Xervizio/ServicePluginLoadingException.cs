using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    public class ServicePluginLoadingException : Exception {
        const string DEFAULT_MESSAGE = "Unable to load service plugin.";
        public ServicePluginLoadingException(Exception ex) : base(DEFAULT_MESSAGE, ex) { }
    }
}
