using System;
using System.Drawing;

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
    }
}
