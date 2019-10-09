using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Color is an invalid format
    /// </summary>
    public class InvalidAnimationActionException : Exception {
        public InvalidAnimationActionException() {
        }

        public InvalidAnimationActionException(string message)
            : base(message) {
        }

        public InvalidAnimationActionException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
