using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    
    [Serializable]
    public class ServicePluginLoadingException : Exception {
        const string DEFAULT_MESSAGE = "Unable to load service plugin.";

        public ServicePluginLoadingException() : base(DEFAULT_MESSAGE) { }
        public ServicePluginLoadingException(string message) : base(message) { }
        public ServicePluginLoadingException(Exception ex) : base(DEFAULT_MESSAGE, ex) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected ServicePluginLoadingException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
}
