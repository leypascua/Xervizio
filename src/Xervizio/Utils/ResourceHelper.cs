using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xervizio {
    public static class ResourceHelper {

        public static Stream GetResourceStream(Assembly assembly, string name) {
            assembly.ShouldNotBeNull("assembly");
            name.ShouldNotBeEmpty("name");

            return assembly.GetManifestResourceStream(name);
        }

        public static string GetResourceString(Assembly assembly, string name) {
            using (var stream = GetResourceStream(assembly, name))
            using (var streamReader = new StreamReader(stream)) {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Searches for the first occurrence of a resource with the supplied resource name from all 
        /// loaded assemblies in the appdomain.
        /// </summary>
        /// <param name="currentDomain"></param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string GetResourceString(this AppDomain currentDomain, string resourceName) {
            resourceName.ShouldNotBeEmpty("resourceName");
            var assemblies = currentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                using (var resource = assembly.GetManifestResourceStream(resourceName)) {
                    if (resource == null)
                        continue;

                    using (var reader = new StreamReader(resource)) {
                        return reader.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }

        public static string GetResourceStringFromAssemblyQualifiedName(this AppDomain currentDomain, string assemblyQualifiedResourceName) {
            currentDomain.ShouldNotBeNull("currentDomain");
            assemblyQualifiedResourceName.ShouldNotBeEmpty("assemblyQualifiedResourceName");

            List<string> parts = assemblyQualifiedResourceName.Split(',')
                         .Select(x => x.Trim())
                         .ToList();

            Protect.AgainstInvalidOperation(parts.Count < 2, // at least provide a resource name and an assembly name.
                "Invalid assembly qualified resource name (assembly name missing): {0}".WithTokens(assemblyQualifiedResourceName));

            string name = parts[0];
            string assembly = parts.Count < 2 ? null : parts[1];
            string version = parts.Count < 3 ? null : parts[2];
            string culture = parts.Count < 4 ? null : parts[3];
            string token = parts.Count < 5 ? null : parts[4];

            if (version != null && !version.StartsWith("Version=")) {
                throw new ArgumentException("Invalid version: " + version);
            }

            var containingAssembly = currentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == assembly);
            Protect.AgainstInvalidOperation(containingAssembly.IsNull(),
                "The assembly {0} was not loaded or cannot be found.".WithTokens(assembly));

            return GetResourceString(containingAssembly, name);

        }


    }
}
