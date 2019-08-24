using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Tried to create a renderer that's not supported
    /// </summary>
    public class InvalidRendererException : Exception {
        public InvalidRendererException() {
        }

        public InvalidRendererException(string message)
            : base(message) {
        }

        public InvalidRendererException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
