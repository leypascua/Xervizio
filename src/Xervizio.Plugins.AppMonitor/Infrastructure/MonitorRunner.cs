using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xervizio.Plugins.AppMonitor.Configuration;
using System.ComponentModel;
using System.Threading;

namespace Xervizio.Plugins.AppMonitor.Infrastructure {
    
    public class MonitorRunner {

        public MonitorRunner(AppMonitorConfiguration configuration) {
            _configuration = configuration;
        }

        public virtual void Run(Action<MonitorTargetItem> onStarted, Action<MonitorTargetItem, Exception> onError) {
            var workers = new List<BackgroundWorker>();

            foreach (MonitorTargetItem item in _configuration.MonitorTargets) {
                var appMonitor = new AppMonitor(_configuration, item.Name);
                appMonitor.OnNotifyError += onError;
                onStarted(item);

                var worker = new BackgroundWorker();
                worker.DoWork += (sender, e) => {
                    try {
                        appMonitor.Monitor();
                    }
                    catch (Exception ex) {
                        onError(item, ex);
                    }
                };

                workers.Add(worker);
                worker.RunWorkerAsync();
            }

            while (workers.Any(x => x.IsBusy)) {
                Thread.Sleep(300);
            }

            foreach (var worker in workers) {
                worker.Dispose();
            }

            workers.Clear();            
            workers = null;
        }
        

        private readonly AppMonitorConfiguration _configuration;
    }
}
