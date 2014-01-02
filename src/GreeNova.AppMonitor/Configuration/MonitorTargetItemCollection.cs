using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GreeNova.AppMonitor.Configuration {
    public class MonitorTargetItemCollection : ConfigurationElementCollection {

        public MonitorTargetItem this[int index] {
            get {
                return base.BaseGet(index) as MonitorTargetItem;
            }
            set {
                if (base.BaseGet(index) != null) {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new MonitorTargetItem this[string name] {
            get {
                return base.BaseGet(name) as MonitorTargetItem;
            }
            set {
                throw new NotImplementedException();
            }
        }
        

        protected override ConfigurationElement CreateNewElement() {
            return new MonitorTargetItem();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((MonitorTargetItem)element).Name;
        }

    }
}
