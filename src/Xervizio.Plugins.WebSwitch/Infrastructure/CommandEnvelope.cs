using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Utils;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {
    public class CommandEnvelope {
        public string CommandTypeFullName { get; set; }
        public string CommandJson { get; set; }

        public virtual ApplicationCommand GetCommandInstance() {
            Type commandType = CommandTypeFullName.ToType();
            var instance = CommandJson.DeserializeFromJsonAs(commandType);
            return (ApplicationCommand)instance;
        }
    }
}
