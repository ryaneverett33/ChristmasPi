using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    public class UnsupportedHardwareException : Exception {
        public UnsupportedHardwareException() {
        }

        public UnsupportedHardwareException(string message)
            : base(message) {
        }

        public UnsupportedHardwareException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
