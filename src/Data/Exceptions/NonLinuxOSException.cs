using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    public class NonLinuxOSException : Exception {
        public NonLinuxOSException() : base("Non-linux OS not supported") {
        }

        public NonLinuxOSException(string message)
            : base(message) {
        }

        public NonLinuxOSException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
