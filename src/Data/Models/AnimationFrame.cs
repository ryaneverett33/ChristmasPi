using System;
using System.Drawing;

namespace ChristmasPi.Data.Models {
    public class AnimationFrame {
        public FrameAction Action;
        public Color[] Colors;
        public Color[][] Colors2D;

        public AnimationFrame(FrameAction action) : this(action, (Color[])null) { }
        public AnimationFrame(FrameAction action, Color[] values) {
            Action = action;
            Colors = values;
        }
        public AnimationFrame(FrameAction action, Color[][] values) {
            Action = action;
            Colors2D = values;
        }
    }
    public enum FrameAction {
        Update,             // Update the current frame with new values
        Update2D,           // Update the current 2D frame with new values
        Sleep,              // Let the renderer sleep
        Blank               // do nothing, wait for time sync
    }
}
