using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Tried to create a renderer that's not supported
    /// </summary>
    public class InvalidSetupActionException : Exception {
        public InvalidSetupActionException() {
        }

        public InvalidSetupActionException(string message)
            : base(message) {
        }

        public InvalidSetupActionException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
