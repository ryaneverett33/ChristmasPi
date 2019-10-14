using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Util {
    public class FrameHelpers {
        /// <summary>
        /// Returns an array of a repeated object
        /// </summary>
        /// <typeparam name="T">Type of object being repeated</typeparam>
        /// <param name="obj">The object to repeat</param>
        /// <param name="count">The number of times to repeat the object</param>
        /// <returns>A filled out array with one object repeated many times</returns>
        public static T[] repeat<T>(T obj, int count) {
            T[] arr = new T[count];
            for (int i = 0; i < count; i++) {
                arr[i] = obj;
            }
            return arr;
        }
    }
}
