using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Xervizio.Utils {

    public delegate void MessageReceivedEvent(object sender, MessageReceivedEventArgs e);
    
    public class FileSystemMessageMonitor {

        public FileSystemMessageMonitor(string pathToWatch = "", string filterExpression = "*.*", int pollingInterval = 1000) {
            PathToWatch = pathToWatch.GetValueOrDefault(AppDomain.CurrentDomain.BaseDirectory);
            FilterExpression = filterExpression;
            PollingInterval = pollingInterval;            
        }

        public event MessageReceivedEvent OnMessageReceived;
        public string PathToWatch { get; set; }
        public string FilterExpression { get; set; }
        public int PollingInterval { get; set; }

        public virtual void Enable() {
            Protect.AgainstInvalidOperation(!Directory.Exists(PathToWatch), 
                "The path {0} does not exist or access is denied", PathToWatch);

            if (_timer.Exists()) return;

            _timer = new Timer(PollingInterval);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Enabled = true;
            _timer.Start();
        }

        public virtual void Disable() {
            if (_timer.IsNull()) return;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            Timer t = (Timer)sender;
            RunTimer(t, false);

            var messages = ReadMessages();
            foreach (var msg in messages) {
                if (OnMessageReceived != null) {
                    OnMessageReceived(this, msg);
                }
                msg.FileData.Delete();
            }

            RunTimer(t, true);
        }

        private IEnumerable<MessageReceivedEventArgs> ReadMessages() {
            var results = new List<MessageReceivedEventArgs>();

            var files = Directory.EnumerateFiles(PathToWatch, FilterExpression);
            foreach (var f in files) {
                results.Add(new MessageReceivedEventArgs(new FileInfo(f)));
            }

            return results.OrderBy(x => x.FileData.CreationTimeUtc);
        }

        private void RunTimer(Timer t, bool start) {
            t.Enabled = start;

            if (start) {
                t.Start();
            }
            else {
                t.Stop();
            }
        }

        private Timer _timer = null;
    }

    public class MessageReceivedEventArgs : EventArgs {
        public MessageReceivedEventArgs(FileInfo fi) {
            FileData = fi;    
        }

        public FileInfo FileData { get; private set; }
    }
}
