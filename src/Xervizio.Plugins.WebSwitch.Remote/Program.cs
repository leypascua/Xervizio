using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using Xervizio.Plugins.WebSwitch.Commands;
using Xervizio.Plugins.WebSwitch.Infrastructure;
using Xervizio.Plugins.WebSwitch.Services;

namespace Xervizio.Plugins.WebSwitch.Remote {
    class Program {
        private static FileSystemHostSwitchClient _client;
        private static string CommandName = string.Empty;
        private static string PluginName = string.Empty;

        static void Main(string[] args) {
            PrintBanner();

            var config = new FileSystemHostSwitchConfiguration(ConfigurationManager.AppSettings["basePath"]);
            _client = new FileSystemHostSwitchClient(config);
            
            Dispatch(args);
        }

        static void Dispatch(string[] args) {
            var optionSet = BuildOptions();            
            var extraArgs = optionSet.Parse(args);
            ExecuteCommand(CommandName);
        }

        private static OptionSet BuildOptions() {
            var optionSet = new OptionSet {
                { "shutdown", "Shuts down the service host process", v => SetupCommand(typeof(ShutdownHostCommand).ToQualifiedAssemblyName(), "SERVICEHOST_1n$t4nz") },
                { "start=", "Starts a plugin that is identified by the supplied value", v => SetupCommand(typeof(StartPluginCommand).ToQualifiedAssemblyName(), v) },
                { "stop=", "Stops a plugin that is identified by the supplied value", v => SetupCommand(typeof(StopPluginCommand).ToQualifiedAssemblyName(), v) },                
            };

            return optionSet;
        }

        private static void SetupCommand(string commandName, string pluginName) {
            CommandName = commandName;
            PluginName = pluginName;
        }

        private static void ExecuteCommand(string commandName) {
            if (CommandName.IsNullOrEmpty() || PluginName.IsNullOrEmpty()) {
                var options = BuildOptions();
                var writer = new StringWriter();
                options.WriteOptionDescriptions(writer);
                Console.WriteLine(writer.ToString());
                Environment.Exit(-99);                                
            }
            
            var createCommand = new Dictionary<string, Func<ApplicationCommand>> {
                { typeof(ShutdownHostCommand).ToQualifiedAssemblyName(), () => new ShutdownHostCommand() },
                { typeof(StartPluginCommand).ToQualifiedAssemblyName(), () => new StartPluginCommand { PluginName = Program.PluginName } },
                { typeof(StopPluginCommand).ToQualifiedAssemblyName(), () => new StopPluginCommand { PluginName = Program.PluginName } },
            };
            
            Console.WriteLine(_client.ExecuteCommand(createCommand[commandName]()));
        }

        private static void PrintBanner() {
            Console.WriteLine(string.Empty);
            Console.WriteLine("Xervizio Host Remote Console");
            Console.WriteLine("Originally developed by leypascua. Contributions are welcome.");
            Console.WriteLine(string.Empty);
        }
    }
}
