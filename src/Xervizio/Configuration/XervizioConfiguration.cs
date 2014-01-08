using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace Xervizio.Configuration {
    public interface XervizioConfiguration {
        string ServiceName { get; set; }
        string DisplayName { get; set; }
        string ServiceDescription { get; set; }
        string PluginLocation { get; set; }
        bool ShutdownBackdoorEnabled { get; set; }
        bool IsOnline { get; }
    }

    public static class XervizioConfigurationExtensions {
        public static string GetPluginsPath(this XervizioConfiguration config) {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.PluginLocation);
        }
    }


    public abstract class XervizioConfigurationContext {

        static XervizioConfigurationContext() {
            Current = Factory();
        }

        public static Func<XervizioConfigurationContext> Factory = () => new XervizioConfigurationSectionContext();

        public static XervizioConfigurationContext Current { get; private set; }

        public abstract XervizioConfiguration Settings { get; }

        class XervizioConfigurationSectionContext : XervizioConfigurationContext {
            public override XervizioConfiguration Settings {
                get { return XervizioConfigurationSection.Current; }
            }
        }
    }

    public class XervizioConfigurationSection : ConfigurationSection, XervizioConfiguration {

        static XervizioConfigurationSection() {
            var configSection = (XervizioConfigurationSection)ConfigurationManager.GetSection("xervizio");
            Current = configSection ?? new XervizioConfigurationSection();
        }

        public static XervizioConfigurationSection Current { get; private set; }

        [ConfigurationProperty(SERVICENAME_CONFIGKEY, DefaultValue = "Xervizio.Host.Standalone")]
        public string ServiceName {
            get {
                return base[SERVICENAME_CONFIGKEY].ToString();
            }
            set {
                base[SERVICENAME_CONFIGKEY] = value;
            }
        }

        [ConfigurationProperty(DISPLAYNAME_CONFIGKEY, DefaultValue = "Xervizio Standalone Host")]
        public string DisplayName {
            get {
                return base[DISPLAYNAME_CONFIGKEY].GetValueOrDefault<string>();
            }
            set {
                base[DISPLAYNAME_CONFIGKEY] = value;
            }
        }

        [ConfigurationProperty(SERVICEDESCRIPTION_CONFIGKEY, DefaultValue = "A plug-in based service host for running background services on very restrictive environments.")]
        public string ServiceDescription {
            get {
                return base[SERVICEDESCRIPTION_CONFIGKEY].GetValueOrDefault<string>();
            }
            set {
                base[SERVICENAME_CONFIGKEY] = value;
            }
        }

        [ConfigurationProperty(PLUGINLOCATION_CONFIGKEY, DefaultValue = "Plugins")]
        public string PluginLocation {
            get {
                return base[PLUGINLOCATION_CONFIGKEY].GetValueOrDefault<string>();
            }
            set {
                base[PLUGINLOCATION_CONFIGKEY] = value;
            }
        }

        [ConfigurationProperty(SHUTDOWNBACKDOORENABLED_CONFIGKEY, DefaultValue = false)]
        public bool ShutdownBackdoorEnabled {
            get {
                return bool.Parse(base[SHUTDOWNBACKDOORENABLED_CONFIGKEY].ToString());
            }
            set {
                base[SHUTDOWNBACKDOORENABLED_CONFIGKEY] = value;
            }
        }

        [ConfigurationProperty(ISONLINE_CONFIGKEY, DefaultValue = true)]
        public bool IsOnline {
            get {
                return bool.Parse(base[ISONLINE_CONFIGKEY].ToString());
            }
            set {
                base[ISONLINE_CONFIGKEY] = value;
            }
        }

        const string SERVICENAME_CONFIGKEY = "serviceName";
        const string DISPLAYNAME_CONFIGKEY = "displayName";
        const string SERVICEDESCRIPTION_CONFIGKEY = "serviceDescription";
        const string PLUGINLOCATION_CONFIGKEY = "pluginLocation";
        const string SHUTDOWNBACKDOORENABLED_CONFIGKEY = "shutdownBackdoorEnabled";
        const string ISONLINE_CONFIGKEY = "isOnline";
    }
}
