using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Utils;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {
    public class CommandEnvelope {

        public static CommandEnvelope BuildFrom(string json) {
            return json.DeserializeFromJsonAs<CommandEnvelope>();
        }

        public CommandEnvelope() { }

        public CommandEnvelope(ApplicationCommand command) {
            CommandTypeFullName = command.GetType().ToQualifiedAssemblyName();
            CommandJson = command.ToJson();
        }

        public string CommandTypeFullName { get; set; }
        public string CommandJson { get; set; }

        public virtual ApplicationCommand GetCommandInstance() {
            Type commandType = CommandTypeFullName.ToType();
            var instance = CommandJson.DeserializeFromJsonAs(commandType);
            return (ApplicationCommand)instance;
        }

        public virtual TCmd UnpackCommandInstance<TCmd>() where TCmd : ApplicationCommand {
            return (TCmd)GetCommandInstance();
        }
    }
}
