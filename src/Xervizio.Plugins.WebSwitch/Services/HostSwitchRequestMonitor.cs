using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Xervizio.Utils;

namespace Xervizio.Plugins.WebSwitch.Services {
    
    public class HostSwitchRequestMonitor {
        
        public HostSwitchRequestMonitor(FileSystemHostSwitchConfiguration config, string extensionFilename) {
            _messageMonitor = new FileSystemMessageMonitor(config.BuildHostedRequestPath(), "*{0}".WithTokens(extensionFilename), 250);
            _messageMonitor.OnMessageReceived += OnMessageReceived;
        }

        public event HostSwitchRequestReceivedEvent RequestReceived;        

        public HostSwitchRequestMonitor Enable() {
            _messageMonitor.Enable();
            return this;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
            if (RequestReceived == null) return;
            if (!e.FileData.Name.Contains(".req.")) return;
            using (var txtReader = e.FileData.OpenText()) {
                RequestReceived(this, new HostSwitchRequestReceivedEventArgs(e.FileData.FullName, txtReader.ReadToEnd()));
            }
        }
        
        private FileSystemMessageMonitor _messageMonitor;
        
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
