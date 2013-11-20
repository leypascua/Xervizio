using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio.Host.Standalone {
    using System.Reflection;
    using log4net;
    using Utils;

    public class SimpleLogger : MarshalByRefObject, ILogger {

        static SimpleLogger() {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
        }

        public void Info(string message, params object[] args) {
            _logger.Info(FormatString(message, args));
        }

        public void Warn(string message, params object[] args) {
            _logger.Warn(FormatString(message, args));
        }

        public void Error(string message, params object[] args) {
            _logger.Error(FormatString(message, args));
        }

        public void Fatal(string message, params object[] args) {
            string finalMessage = FormatString(message, args);
            _logger.Fatal(finalMessage);
            Console.WriteLine(finalMessage);
        }

        public void Fatal(Exception error) {
            _logger.Fatal("Fatal exception occurred: ", error);
        }

        static string FormatString(string input, params object[] args) {
            return string.Format(input, args);
        }

        static void ThreadSafe(Action activity) {
            lock (_syncLock) {
                activity();
            }
        }

        ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static object _syncLock = new object();
    }
}
