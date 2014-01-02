using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;

namespace Xervizio.Plugins.WebSwitch.Services {
    public class FileSystemHostSwitchClient {
        
        public const string REQUEST_EXTENSION_FILENAME = ".xvzio";
        private FileSystemHostSwitchConfiguration _config;

        public FileSystemHostSwitchClient(FileSystemHostSwitchConfiguration config) {
            _config = config;
        }

        public virtual string ExecuteCommand(ApplicationCommand command) {
            var envelope = new CommandEnvelope(command);
            string requestId = "[{0}@{1}]{2}.{3}".WithTokens(
                Environment.UserName, Environment.MachineName, command.GetType().Name, DateTime.UtcNow.ToString("yyyyMMddThhmmssfff"));

            // build request file name
            // format: [{username}@{host}].{yyyyMMddThhmmssnnn}.req.xvzio            
            string requestFilename = "{0}.req{1}".WithTokens(requestId, REQUEST_EXTENSION_FILENAME);

            // build response file name
            // format: [{username}@{host}].{yyyyMMddThhmmssnnn}.res.xvzio
            string responseFilename = "{0}.res{1}".WithTokens(requestId, REQUEST_EXTENSION_FILENAME);
                                    
            string responsePath = _config.BuildHostedResponsePath();
            string responseFilePath = Path.Combine(responsePath, responseFilename);

            // serialize command
            SerializeAndSend(envelope, Path.Combine(_config.BuildHostedRequestPath(), requestFilename));

            // wait for response
            int retryCount = 0;
            while (!ResponseReceived(responseFilePath)) {
                Protect.Against<TimeoutException>(retryCount == 10, "Execution of {0} timed out.", command.GetType().FullName);
                Thread.Sleep(1000 * 3); // retry every 3 seconds
                retryCount++;
            }

            // deserialize response
            return DeserializeResponse(responseFilePath, command);
        }

        private bool ResponseReceived(string responseFilePath) {
            return File.Exists(responseFilePath);
        }

        private void SerializeAndSend(CommandEnvelope envelope, string requestFilePath) {
            File.WriteAllText(requestFilePath, envelope.ToJson());
        }

        private string DeserializeResponse(string responseFilePath, ApplicationCommand command) {
            string json = File.ReadAllText(responseFilePath);
            bool isShutdownCommand = command is ShutdownHostCommand;
            bool rethrowError = json.Contains(typeof(ThrowHostSwitchExceptionCommand).FullName) && !isShutdownCommand;
            var envelope = CommandEnvelope.BuildFrom(json);
    
            if (rethrowError) {
                var error = envelope.UnpackCommandInstance<ThrowHostSwitchExceptionCommand>();
                throw error.OriginalException;
            }
                        
            var response = envelope.UnpackCommandInstance<SuccessfulExecutionResponseCommand>();
            string result = response.Message ?? string.Empty;

            return result;
        }
    }
}
