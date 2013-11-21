using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xervizio {
    public static class InstanceHelper {

        [DebuggerStepThrough]
        public static object CreateObjectInstance(string typeName, params object[] constructorArgs) {
            var type = Type.GetType(typeName);
            if (type == null) return null;

            return CreateObjectInstance(type, constructorArgs);
        }

        [DebuggerStepThrough]
        public static object CreateObjectInstance(Type instanceType, params object[] constructorArgs) {
            try {
                return Activator.CreateInstance(instanceType, constructorArgs);
            }
            catch (TargetInvocationException ex) {
                throw ex.GetBaseException();
            }
        }

        [DebuggerStepThrough]
        public static T CreateObjectInstance<T>(params object[] constructorArgs) where T : class {
            return (T)CreateObjectInstance(typeof(T), constructorArgs);
        }
    }
}
