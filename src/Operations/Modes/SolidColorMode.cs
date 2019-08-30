using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;

namespace ChristmasPi.Operations.Modes {
    public class SolidColorMode : IOperationMode {
        public string Name => "SolidColorMode";
        public void Activate() {
            Console.WriteLine("Activated Solid Color Mode");
        }
        public void Deactivate() {
            Console.WriteLine("Deactivate Solid Color Mode");
        }
    }
}
