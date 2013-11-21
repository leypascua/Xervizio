using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xervizio {
    public static class EnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> lambda) {
            foreach (var item in enumerable) {
                lambda(item);
            }
        }

        public static bool IsEmpty<T>(this IEnumerable<T> items) {
            return items.IsNull() || !items.Any();
        }

        public static string ToCsv<T>(this IEnumerable<T> enumerable) {
            Protect.AgainstNullArgument(enumerable, "enumerable");

            if (!enumerable.Any())
                return string.Empty;

            var result = new StringBuilder();

            foreach (T item in enumerable) {
                try {
                    if (result.Length > 0) {
                        result.AppendFormat(", {0}", item);
                    }
                    else
                        result.Append(item);
                }
                catch { }
            }

            return result.ToString();
        }
    }
}
