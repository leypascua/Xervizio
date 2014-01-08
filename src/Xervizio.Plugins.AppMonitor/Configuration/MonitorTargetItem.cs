using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Xervizio.Plugins.AppMonitor.Configuration {
    public class MonitorTargetItem : ConfigurationElement {

        [ConfigurationProperty(NAME_KEY, IsRequired = true)]
        public string Name {
            get {
                return this[NAME_KEY] as string;
            }
        }

        [ConfigurationProperty(APPLICATIONURL_KEY, IsRequired = true)]
        public string ApplicationUrl {
            get {
                return this[APPLICATIONURL_KEY] as string;
            }
        }

        [ConfigurationProperty(ERRORNOTIFICATIONMAILRECIPIENTS_KEY, IsRequired = true)]
        public string ErrorNotificationMailRecipients {
            get {
                return this[ERRORNOTIFICATIONMAILRECIPIENTS_KEY] as string;
            }
        }

        [ConfigurationProperty(TIMEOUTINSECONDS_KEY, DefaultValue = 45)]
        public int TimeOutInSeconds {
            get {
                return int.Parse(this[TIMEOUTINSECONDS_KEY].ToString());
            }
        }

        const string NAME_KEY = "name";
        const string APPLICATIONURL_KEY = "applicationUrl";
        const string ERRORNOTIFICATIONMAILRECIPIENTS_KEY = "errorNotificationMailRecipients";
        const string TIMEOUTINSECONDS_KEY = "timeoutInSeconds";
    }
}
