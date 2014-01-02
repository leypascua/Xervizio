using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {
    public class CommandContext {
        public object Result { get; set; }

        public virtual T GetResult<T>() {
            return Result.Exists() ? (T)Result : default(T);
        }
    }
}
