using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Xervizio.Configuration {
    public static class ObjectExtensions {
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
    }
}
