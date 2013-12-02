using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {
    public static class CommandRegistry {
        public static void Register<T>(Func<ICommandProcessor<T>> processorFactory) where T : ApplicationCommand {
            Protect.AgainstInvalidOperation(_registry.ContainsKey(typeof(T)),
                "A processor for the command {0} was already registered.", typeof(T).FullName);

            _registry.Add(typeof(T), processorFactory);
        }

        public static CommandProcessorProxy GetCommandProcesor(ApplicationCommand command) {
            Type commandType = command.GetType();

            Protect.AgainstInvalidOperation(!_registry.ContainsKey(commandType),
                "An ICommandProcessor for {0} cannot be found or was not registered.", commandType.FullName);

            return new CommandProcessorProxy(_registry[commandType](), command);
        }

        static Dictionary<Type, Func<CommandProcessor>> _registry = new Dictionary<Type, Func<CommandProcessor>>();
    }
}
