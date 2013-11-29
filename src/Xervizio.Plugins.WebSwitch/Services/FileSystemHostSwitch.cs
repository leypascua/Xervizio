using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Api;

namespace Xervizio.Plugins.WebSwitch.Services {
    public class FileSystemHostSwitch {
        private FileSystemHostSwitchConfiguration _config;
        private FileSystemWatcher _fsw;

        public FileSystemHostSwitch(FileSystemHostSwitchConfiguration config) {
            _config = config;
            _fsw = new FileSystemWatcher(_config.BuildHostedRequestPath());
        }

        public FileSystemHostSwitch Start() {
            Reset();
            _fsw.Filter = "*.xvzo";
            _fsw.Created += OnRequestReceived;
            _fsw.EnableRaisingEvents = true;
            return this;
        }

        void OnRequestReceived(object sender, FileSystemEventArgs e) {
            // deserialize file

            // execute the command
        }

        private void Reset() {
            // delete files in request path

            // delete files in response path
        }
    }

    public class FileSystemHostSwitchConfiguration {
        public string RequestPath { get; set; }
        public string ResponsePath { get; set; }
        public HostController HostController { get; set; }
        public PluginsController PluginsController { get; set; }

        public virtual string BuildHostedRequestPath() {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, RequestPath);
        }
    }
}
