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
}
