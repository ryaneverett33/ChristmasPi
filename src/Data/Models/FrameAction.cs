using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public enum FrameAction {
        Update,             // Update the current frame with new values
        Update2D,           // Update the current 2D frame with new values
        Sleep,              // Let the renderer sleep
        Blank               // do nothing, wait for time sync
    }
}
