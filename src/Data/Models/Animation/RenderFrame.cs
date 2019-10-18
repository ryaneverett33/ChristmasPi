using System;
using System.Drawing;
using ChristmasPi.Data;

namespace ChristmasPi.Data.Models.Animation {
    public class RenderFrame {
        public FrameAction Action;
        public ColorList Colors;
        
        /// <summary>
        /// Creates a new Render Frame with a specific action
        /// </summary>
        /// <param name="action">The action of this frame</param>
        /// <seealso cref="FrameAction"/>
        /// <remarks>If creating an update RenderFrame, use RenderFrame(Color[] colors)</remarks>
        public RenderFrame(FrameAction action) : this(action, null) { }

        /// <summary>
        /// Creates a new Update Render Frame with the specified colors
        /// </summary>
        /// <param name="colors">The colors to use when rendering</param>
        public RenderFrame(ColorList colors) : this(FrameAction.Update, colors) { }

        /// <summary>
        /// Creates a new Render Frame
        /// </summary>
        /// <param name="action">The render action</param>
        /// <param name="colors">Colors to use during the render</param>
        public RenderFrame(FrameAction action, ColorList colors) {
            Action = action;
            Colors = colors;
        }

        public static RenderFrame operator+(RenderFrame a, RenderFrame other) {
            if (a.Action == FrameAction.Sleep || a.Action == FrameAction.Blank) {
                if (other.Action == FrameAction.Sleep || other.Action == FrameAction.Sleep)
                    return a;
                else
                    return other;
            }
            else if (other.Action == FrameAction.Sleep || other.Action == FrameAction.Sleep)
                return a;
            ColorList newlist = new ColorList();
            ColorValue off = new ColorValue(Constants.COLOR_OFF);
            if (a.Colors.Count != other.Colors.Count)
                throw new Exception("Cannot add two render frames together, they are different sizes");
            for (int i = 0; i < a.Colors.Count; i++) {
                if (a.Colors[i] == off)
                    newlist.Add(other.Colors[i]);
                else if (other.Colors[i] == off)
                    newlist.Add(a.Colors[i]);
                else
                    throw new Exception("Cannot add two render frames together, would result in color clashing");
            }
            return new RenderFrame(newlist);
        }
    }
}
