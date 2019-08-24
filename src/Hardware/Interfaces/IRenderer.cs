using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Hardware.Interfaces {
    public interface IRenderer {
        void Render(IRenderer obj);
        void SetLEDColor();
    }
}
