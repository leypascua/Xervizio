﻿using System;
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

        public virtual void ExecuteCommand(ApplicationCommand command) {
            var envelope = new CommandEnvelope(command);
            string requestId = "[{0}@{1}]{2}.{3}".WithTokens(
                Environment.UserName, Environment.MachineName, command.GetType().Name, DateTime.UtcNow.ToString("yyyyMMddThhmmssfff"));

            // build request file name
            // format: [{username}@{host}].{yyyyMMddThhmmssnnn}.req.xvzio            
            string requestFilename = "{0}.req{1}".WithTokens(requestId, REQUEST_EXTENSION_FILENAME);

            // build response file name
            // format: [{username}@{host}].{yyyyMMddThhmmssnnn}.res.xvzio
            string responseFilename = "{0}.res{1}".WithTokens(requestId, REQUEST_EXTENSION_FILENAME);

            // start file system watcher on the response path
            string responseFilePath = string.Empty;
            string responsePath = _config.BuildHostedResponsePath();
            var fsw = new FileSystemWatcher(responsePath, responseFilename);
            fsw.Created += (sender, args) => {
                responseFilePath = args.FullPath;
            };
            fsw.EnableRaisingEvents = true;

            // serialize command
            SerializeAndSend(envelope, Path.Combine(_config.BuildHostedRequestPath(), requestFilename));

            // wait for response
            int retryCount = 0;
            while (responseFilePath.IsNullOrEmpty()) {
                if (command is ShutdownHostCommand) {
                    return;
                }

                Protect.Against<TimeoutException>(retryCount == 2, "Execution of {0} timed out.", command.GetType().FullName);
                Thread.Sleep(2000);
                retryCount++;
            }

            // deserialize response
            DeserializeResponse(responseFilePath, command);
        }

        private void SerializeAndSend(CommandEnvelope envelope, string requestFilePath) {
            File.WriteAllText(requestFilePath, envelope.ToJson());
        }

        private void DeserializeResponse(string responseFilePath, ApplicationCommand command) {
            string json = File.ReadAllText(responseFilePath);
            bool isShutdownCommand = command is ShutdownHostCommand;
            bool rethrowError = json.Contains(typeof(ThrowHostSwitchExceptionCommand).FullName) && !isShutdownCommand;

            if (rethrowError) {
                var error = json.DeserializeFromJsonAs<ThrowHostSwitchExceptionCommand>();
                throw error.OriginalException;
            }
        }
    }
}