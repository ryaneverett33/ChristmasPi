using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public enum FrameAction {
        /// <summary>
        /// Update the current frame with new values
        /// </summary>
        Update,
        /// <summary>
        /// Update the current 2D frame with new values
        /// </summary>
        Update2D,
        /// <summary>
        /// Let the renderer sleep
        /// </summary>
        Sleep,
        /// <summary>
        /// do nothing, wait for time sync
        /// </summary>
        Blank
    }
}
