using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Xervizio {

    [DebuggerNonUserCode]
    public static class ObjectExtensions {

        public static bool IsNull(this object instance) {
            return instance == null;
        }

        public static bool Exists(this object instance) {
            return !instance.IsNull();
        }

        public static void ShouldNotBeNull(this object instance, string argName = "Argument") {
            Protect.AgainstNullArgument(instance, argName);
        }

        public static bool InstanceOf<T>(this object instance) where T : class {
            if (instance.IsNull()) return false;
            return instance.GetType() == typeof(T);
        }

        public static string ToJson(this object instance) {
            instance.ShouldNotBeNull();
            return JsonConvert.SerializeObject(instance);
        }

        public static T GetValueOrDefault<T>(this object sourceValue, T defaultValue = default(T)) {
            if (sourceValue == null) return defaultValue;
            try {
                T typedValue = (T)sourceValue;
                if (sourceValue.GetType().IsAssignableFrom(typeof(IEquatable<T>))) {
                    return ((IEquatable<T>)typedValue).Equals(default(T)) ? defaultValue : typedValue;
                }

                return typedValue;
            }
            catch {
                return defaultValue;
            }
        }

        public static IDictionary<string, string> ToKeyValues(this object instance) {
            if (instance == null) {
                return new Dictionary<string, string>();
            }

            var properties = TypeDescriptor.GetProperties(instance);

            var hash = new Dictionary<string, string>(properties.Count);

            foreach (PropertyDescriptor descriptor in properties) {
                var key = descriptor.Name;
                var value = descriptor.GetValue(instance);

                if (value != null) {
                    hash.Add(key, value.ToString());
                }
                else {
                    hash.Add(key, string.Empty);
                }
            }

            return hash;
        }


    }
}
