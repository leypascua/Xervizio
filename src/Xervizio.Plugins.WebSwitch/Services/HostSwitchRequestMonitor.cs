using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Xervizio.Plugins.WebSwitch.Services {
    
    public class HostSwitchRequestMonitor {
        private FileSystemHostSwitchConfiguration _config;
        private string _extensionFilename;
        private Timer _timer;

        public HostSwitchRequestMonitor(FileSystemHostSwitchConfiguration config, string extensionFilename) {
            _config = config;
            _hostedRequestPath = _config.BuildHostedRequestPath();
            _filterPattern = "*{0}".WithTokens(extensionFilename);
            _extensionFilename = extensionFilename;
            _timer = new Timer(150);
            _timer.Elapsed += OnTimerElapsed;
            RequestReceived += (s, e) => { };
        }

        public event HostSwitchRequestReceivedEvent RequestReceived;
        private string _filterPattern;
        private string _hostedRequestPath;

        public HostSwitchRequestMonitor Enable() {
            RunTimer(_timer, true);
            return this;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            Timer t = (Timer)sender;
            RunTimer(t, false);

            var requestFiles = Directory.EnumerateFiles(_hostedRequestPath, _filterPattern);
            foreach (var file in requestFiles) {
                if (!file.Contains(".req.")) continue;
                string fullPath = Path.Combine(_hostedRequestPath, file);
                string requestJson = File.ReadAllText(fullPath);
                RequestReceived(this, new HostSwitchRequestReceivedEventArgs(fullPath, requestJson));
                File.Delete(file);
            }

            RunTimer(t, true);
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
    }

    public delegate void HostSwitchRequestReceivedEvent(object sender, HostSwitchRequestReceivedEventArgs e);

    public class HostSwitchRequestReceivedEventArgs : EventArgs {
        public HostSwitchRequestReceivedEventArgs(string fullPath, string requestJson) {
            FullPath = fullPath;
            RequestJson = requestJson;
        }

        public string FullPath { get; private set; }
        public string RequestJson { get; private set; }
    }
}
