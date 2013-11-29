using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {

    public interface CommandProcessor { }
    
    public interface ICommandProcessor<T> : CommandProcessor where T : ApplicationCommand {
        void Process(T args);
    }
}
