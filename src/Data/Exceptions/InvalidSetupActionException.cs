using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Tried to do a setup action that's invalid
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
