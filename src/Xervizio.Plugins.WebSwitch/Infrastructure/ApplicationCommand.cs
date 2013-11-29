using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio.Plugins.WebSwitch.Infrastructure {
    public abstract class ApplicationCommand {

        public ApplicationCommand() {
            Id = Guid.NewGuid();
        }

        public virtual Guid Id { get; private set; }
    }
}
