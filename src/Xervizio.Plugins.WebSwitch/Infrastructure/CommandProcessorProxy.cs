using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {
    public class CommandProcessorProxy {
        private CommandProcessor _processor;
        private ApplicationCommand _args;

        public CommandProcessorProxy(CommandProcessor commandProcessor, ApplicationCommand args) {
            Type processorType = commandProcessor.GetType();
            Protect.AgainstInvalidOperation(!typeof(CommandProcessor).IsAssignableFrom(processorType),
                "Processor must implement ICommandProcessor");

            _processor = commandProcessor;
            _args = args;
        }

        public virtual void Execute() {
            Type commandType = _args.GetType();
            Type commandProcessorGenericType = typeof(ICommandProcessor<>).MakeGenericType(commandType);
            var method = commandProcessorGenericType.GetMethod("Process");
            try {
                method.Invoke(_processor, new[] { _args });
            }
            catch (TargetInvocationException ex) {
                throw ex.InnerException;
            }
        }
    }
}
