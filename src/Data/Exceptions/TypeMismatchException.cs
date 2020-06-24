using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPi.Data.Exceptions {
    class TypeMismatchException : Exception {
        public TypeMismatchException() : base() { }
        public TypeMismatchException(Type offendA, Type offendB) : base($"{offendA} != {offendB}") { }
    }
}
