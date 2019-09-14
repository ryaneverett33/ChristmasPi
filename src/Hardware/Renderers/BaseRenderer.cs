using System;
using System.Collections.Generic;
using System.Drawing;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Data;
using ChristmasPi.Data.Extensions;

namespace ChristmasPi.Hardware.Renderers {
    public abstract class BaseRenderer : IRenderer {
        protected List<Color> ledColors;                  // a list of all the current color values to render, same size as the amount of LEDs
        protected bool colorsChanged = false;

        public int LightCount { get; protected set; }
        public bool AutoRender => false;
        public abstract void Render(IRenderer obj);
        /// <summary>
        /// Sets the LED color at the position and applies color flips if needed
        /// </summary>
        /// <param name="index">Position of the LED</param>
        /// <param name="color">New color for the LED</param>
        public void SetLEDColor(int index, Color color) {
            if (index < 0 || index >= ledColors.Count)
                throw new ArgumentOutOfRangeException("index");
                if (ConfigurationManager.Instance.TreeConfiguration.tree.color.flipGB)
                    ledColors[index] = color.FlipGB();
                else if (ConfigurationManager.Instance.TreeConfiguration.tree.color.flipRB)
                    ledColors[index] = color.FlipRB();
                else if (ConfigurationManager.Instance.TreeConfiguration.tree.color.flipRG)
                    ledColors[index] = color.FlipRG();
                else
                    ledColors[index] = color;
                colorsChanged = true;
        }
        /// <summary>
        /// Sets all the LEDs to the same color
        /// </summary>
        /// <param name="color">the new color</param>
        public void SetAllLEDColors(Color color) {
            for (int i = 0; i < ledColors.Count; i++) {
                SetLEDColor(i, color);
            }
        }
        public abstract void Start();
        public abstract void Stop();
    }
}
