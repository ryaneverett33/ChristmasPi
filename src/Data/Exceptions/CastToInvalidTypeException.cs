using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Exceptions {
    /// <summary>
    /// Attempted to cast value to a different type than enclosing type
    /// </summary>
    public class CastToInvalidTypeException : Exception {
        public CastToInvalidTypeException() {
        }

        public CastToInvalidTypeException(string message)
            : base(message) {
        }

        public CastToInvalidTypeException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
