using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xervizio {

    public enum ServicePluginState {
        Stopped = 0,
        Starting = 1,
        Idle = 2,
        Busy = 3,
        Stopping = 4 
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

    public abstract class HostGateway : ServicePlugin {
        public virtual ServicePluginHost HostContext {
            get { return _hostContext; }
            internal set {
                if (_hostContext == null) {
                    SetHostContext(value);
                }
            }
        }

        private void SetHostContext(ServicePluginHost value) {            
            //HACK: BAD CODE!!! I'm too lazy to study how to renew a lease for a remote 
            //      object, so this is only good until it explodes!!!
            
            _hostContext = value;
            Action keepAlive = () => {
                while (true) {
                    Thread.Sleep(8500);
                    _hostContext.KeepAlive();                    
                }
            };

            keepAlive.BeginInvoke(asyncResult => { }, _hostContext);
        }

        private ServicePluginHost _hostContext = null;
    }
}
