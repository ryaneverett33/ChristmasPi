using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Tried to parse a timestamp of invalid format
    /// </summary>
    public class InvalidTimestampException : Exception {
        public InvalidTimestampException() {
        }

        public InvalidTimestampException(string message)
            : base(message) {
        }

        public InvalidTimestampException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
