using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    public class InvalidOperatingModeException : Exception {
        public InvalidOperatingModeException() {
        }

        public InvalidOperatingModeException(string message)
            : base(message) {
        }

        public InvalidOperatingModeException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
