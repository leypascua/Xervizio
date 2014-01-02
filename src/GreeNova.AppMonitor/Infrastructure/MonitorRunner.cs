using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreeNova.AppMonitor.Configuration;
using System.ComponentModel;

namespace GreeNova.AppMonitor.Infrastructure {
    
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

            while (true) {
                if (!workers.Any(x => x.IsBusy))
                    break;
            }
        }
        

        private readonly AppMonitorConfiguration _configuration;
    }
}
