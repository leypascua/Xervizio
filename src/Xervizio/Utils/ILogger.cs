using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio.Utils {
    public interface ILogger {
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(string message, params object[] args);
        void Error(Exception error);
        void Fatal(string message, params object[] args);
        void Fatal(Exception error);
    }
}
