using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xervizio.Plugins.AppMonitor.Configuration;
using Xervizio.Plugins.AppMonitor.Infrastructure;
using log4net;
using Xervizio;

namespace Xervizio.Plugins.AppMonitor {
    public class AppMonitorServicePlugin : ServicePlugin {

        private readonly static ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);        

        public override void Start() {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            Console.WriteLine("GreeNova AppMonitor(TM)");
            Console.WriteLine("Copyright (C) 2012 GreeNova Philippines Inc. All rights reserved. \n\n");

            _worker = new BackgroundWorker {
                WorkerSupportsCancellation = true
            };

            _worker.DoWork += (s, e) => StartAppMonitor();            
            _worker.RunWorkerAsync();
        }

        public override void Stop() {
            Console.WriteLine("Stopping AppMonitor...");
            _worker.CancelAsync();
            
            while (_worker.IsBusy) {
                Thread.Sleep(1000);
            }

            _worker.Dispose();
            _worker = null;
            Console.WriteLine("AppMonitor stopped.");
        }

        public override ServicePluginState GetState() {
            throw new NotImplementedException();
        }

        private static void StartAppMonitor() {
            Console.WriteLine("Starting AppMonitor...");
            
            while (true) {
                try {
                    RunAppMonitor();
                }
                catch (Exception ex) {
                    Logger.Fatal("AppMonitor failed.", ex);
                    throw;
                }

                Thread.Sleep(1000 * AppMonitorConfiguration.Current.CheckIntervalInSeconds);
            }
        }

        private static void RunAppMonitor() {
            var monitorRunner = new MonitorRunner(AppMonitorConfiguration.Current);
            monitorRunner.Run(OnMonitorStarted, OnMonitorError);
            monitorRunner = null;
        }

        public static void OnMonitorStarted(MonitorTargetItem item) {
            Console.WriteLine("AppMonitor for {0} started.".WithTokens(item.Name));
        }

        public static void OnMonitorError(MonitorTargetItem item, Exception error) {
            Console.WriteLine("AppMonitor for {0} failed. Reason: {1}".WithTokens(item.Name, error.Message));
        }
                
        private BackgroundWorker _worker;
    }
}
