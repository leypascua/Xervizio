using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Xervizio {


    [DebuggerStepThrough]
    public static class Protect {
        
        public static void Against<TException>(bool condition, string message, params object[] args) where TException : Exception {
            if (condition) {
                throw (TException)Activator.CreateInstance(typeof(TException), message.WithTokens(args));
            }
        }

        public static void AgainstInvalidOperation(bool condition, string message, params object[] args) {
            Against<InvalidOperationException>(condition, message.WithTokens(args));
        }

        public static void AgainstNullArgument(object arg, string argName) {
            Protect.Against<ArgumentNullException>(arg == null, "{0} cannot be null.".WithTokens(argName));
        }

    }
}
