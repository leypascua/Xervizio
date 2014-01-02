using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xervizio.Plugins.WebSwitch.Api;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.Services {
    public class FileSystemHostSwitch {
        private FileSystemHostSwitchConfiguration _config;
        private FileSystemWatcher _fsw;

        public FileSystemHostSwitch(FileSystemHostSwitchConfiguration config) {
            _config = config;
            _fsw = new FileSystemWatcher(_config.BuildHostedRequestPath());
        }

        public FileSystemHostSwitch Start() {
            Reset(_config.BuildHostedRequestPath(), _config.BuildHostedResponsePath());
            _fsw.Filter = "*{0}".WithTokens(FileSystemHostSwitchClient.REQUEST_EXTENSION_FILENAME);
            _fsw.Created += (sender, args) => ExecuteCommand(args.FullPath);
            _fsw.EnableRaisingEvents = true;
            return this;
        }

        private void ExecuteCommand(string requestPath) {
            Thread.Sleep(200);

            // deserialize file            
            var envelope = CommandEnvelope.BuildFrom(File.ReadAllText(requestPath));
            ApplicationCommand response = new SuccessfulExecutionResponseCommand();

            // execute the command
            try {
                var processor = CommandRegistry.GetCommandProcesor(envelope.GetCommandInstance());
                var commandContext = processor.Execute();
                response.Message = commandContext.Result as string;
            }
            catch (Exception ex) {
                response = new ThrowHostSwitchExceptionCommand {
                    OriginalException = ex
                };
            }

            string requestFile = Path.GetFileName(requestPath);
            string responseFilename = requestFile.Replace(".req.", ".res.");
            string responsePath = Path.Combine(_config.BuildHostedResponsePath(), responseFilename);
            File.WriteAllText(responsePath, (new CommandEnvelope(response)).ToJson());
        }

        private void Reset(params string[] paths) {
            // delete files in request and response paths
            paths.ForEach(x => {
                if (Directory.Exists(x)) {
                    Directory.Delete(x, true);
                }
                
                Directory.CreateDirectory(x);
            });
        }
    }

    public class FileSystemHostSwitchConfiguration {

        public FileSystemHostSwitchConfiguration(string basePath) {
            BasePath = basePath ?? AppDomain.CurrentDomain.BaseDirectory;
            RequestPath = "Request";
            ResponsePath = "Response";
        }
        
        public string BasePath { get; set; }
        public string RequestPath { get; set; }
        public string ResponsePath { get; set; }
        
        public virtual string BuildHostedRequestPath() {
            string basePath = BasePath ?? AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, RequestPath);
        }

        public virtual string BuildHostedResponsePath() {
            string basePath = BasePath ?? AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, ResponsePath);
        }
    }
}
