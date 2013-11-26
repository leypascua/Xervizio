using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Xervizio.Data {
    
    [Serializable]
    [DataContract]
    public class PluginData {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsRunning { get; set; }
    }
}
