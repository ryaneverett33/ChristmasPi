using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Extensions {
    public static class ArrayExtensions {
        public static bool IsSorted(this Array array) {
            if (array.Length <= 1) return true;
            object first = array.GetValue(0);
            if (first == null) return false;
            if (!(first is IComparable)) throw new ArgumentException($"Can't compare array of type {array.GetValue(0).GetType().FullName}");
            bool? lastDescending = null;
            for (int i = 1; i < array.Length; i++) {
                IComparable a = (IComparable)array.GetValue(i - 1);
                IComparable b = (IComparable)array.GetValue(i);
                if (a == null) return false;
                if (b == null) return false;
                try {
                    switch (a.CompareTo(b)) {
                        case -1:
                            // a < b
                            if (lastDescending == null)
                                lastDescending = false;
                            else if (lastDescending != null && lastDescending == true)
                                return false;
                            break;
                        case 1:
                            // a > b
                            if (lastDescending == null)
                                lastDescending = true;
                            else if (lastDescending != null && lastDescending == false)
                                return false;
                            break;
                        default:
                            continue;
                    }
                }
                catch (ArgumentException) {
                    throw;
                }
            }
            return true;
        }
        public static IEnumerable<T> Subset<T>(this IEnumerable<T> arr, int start, int end) {
            if (start == end)
                return Array.Empty<T>();
            if (end < start)
                throw new ArgumentException("The end index cannot be less than the start index");
            if (end < 0)
                throw new ArgumentException("The end index must be non-negative");
            if (start < 0)
                throw new ArgumentException("The start index must be non-negative");
            int length = end - start;
            Array newArr = Array.CreateInstance(typeof(T), length);
            Array.Copy(arr.ToArray(), start, newArr, end, length);
            return newArr.Cast<T>();
        }
    }
}
