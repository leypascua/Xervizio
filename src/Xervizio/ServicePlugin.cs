using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Xervizio {

    public enum ServicePluginState {
        Stopped = 0,
        Busy = 1,
        Idle = 2
    }

    [InheritedExport]
    public interface IServicePlugin {
        ServicePluginState GetState();
        void Start();
        void Stop();
    }
    
    public abstract class ServicePlugin : MarshalByRefObject, IServicePlugin {
        public abstract void Start();
        public abstract void Stop();
        public abstract ServicePluginState GetState();
    }
}
