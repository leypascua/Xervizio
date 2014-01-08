using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Xervizio.Plugins.AppMonitor.Configuration {
    
    public class AppMonitorConfiguration : ConfigurationSection {

        static AppMonitorConfiguration() {
            Current = ConfigurationManager.GetSection("appMonitor") as AppMonitorConfiguration;
        }

        public static AppMonitorConfiguration Current { get; private set; }

        [ConfigurationProperty(MONITORMAILSENDER_KEY, IsRequired = true)]
        public string MonitorMailSender {
            get { return base[MONITORMAILSENDER_KEY] as string; }
            set { base[MONITORMAILSENDER_KEY] = value; }
        }


        [ConfigurationProperty(CHECKINTERVALINSECONDS_KEY, DefaultValue = 120)]
        public int CheckIntervalInSeconds {
            get {
                return int.Parse(this[CHECKINTERVALINSECONDS_KEY].ToString());
            }
        }

        [ConfigurationProperty(TARGETS_KEY)]
        public MonitorTargetItemCollection MonitorTargets {
            get {
                return this[TARGETS_KEY] as MonitorTargetItemCollection;
            }
        }

        const string MONITORMAILSENDER_KEY = "monitorMailSender";
        const string TARGETS_KEY = "targets";
        const string CHECKINTERVALINSECONDS_KEY = "checkIntervalInSeconds";
    }
}
