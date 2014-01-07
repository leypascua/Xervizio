using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Xervizio.Plugins.AppMonitor.Helpers {

    public static class Protect {

        [DebuggerStepThrough]
        public static void Against<TException>(bool condition, string message) where TException : Exception {
            if (condition) {
                throw (TException)Activator.CreateInstance(typeof(TException), message);
            }
        }

        [DebuggerStepThrough]
        public static void AgainstInvalidOperation(bool condition, string message) {
            Against<InvalidOperationException>(condition, message);
        }

        [DebuggerStepThrough]
        public static void AgainstNullArgument(object arg, string argName) {
            Protect.Against<ArgumentNullException>(arg == null, "{0} cannot be null.".WithTokens(argName));
        }

    }
}
