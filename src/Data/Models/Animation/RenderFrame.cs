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

        public RenderFrame operator+(RenderFrame other) {
            if (this.Action == FrameAction.Sleep || this.Action == FrameAction.Blank) {
                if (other.Action == FrameAction.Sleep || other.Action == FrameAction.Sleep)
                    return this;
                else
                    return other;
            }
            else if (other.Action == FrameAction.Sleep || other.Action == FrameAction.Sleep)
                return this;
            Colorlist newlist = new Colorlist();
            if (this.Colors.Count != other.Colors.Count)
                throw new Exception("Cannot add two render frames together, they are different sizes");
            for (int i = 0; i < this.Colors.Count; i++) {
                if (this.Colors[i] == Constants.COLOR_OFF)
                    newlist.Add(other.Colors[i]);
                else if (other.Colors[i] == Constants.COLOR_OFF)
                    newlist.Add(this.Colors[i]);
                else
                    throw new Exception("Cannot add two render frames together, would result in color clashing");
            }
            return new RenderFrame(newlist);
        }
    }
}
