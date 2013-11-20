using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    public abstract class ServicePlugin : MarshalByRefObject {
        public abstract void Start();
        public abstract void Stop();
    }
}
