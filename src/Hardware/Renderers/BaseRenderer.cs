using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Extensions;
using ChristmasPi.Data.Models.Hardware;
using Serilog;

namespace ChristmasPi.Hardware.Renderers {
    public abstract class BaseRenderer : IRenderer {
        protected Color[] ledColors;                  // a list of all the current color values to render, same size as the amount of LEDs
        protected bool colorsChanged = false;

        public int LightCount { get; protected set; }
        public virtual bool AutoRender => false;
        public virtual bool is2D => false;
        protected bool stopped = true;
        public abstract event BeforeRenderHandler BeforeRenderEvent;
        public abstract event AfterRenderHandler AfterRenderEvent;
        public abstract void Render(IRenderer obj);

        public BaseRenderer(int ledCount) {
            LightCount = ledCount;
            ledColors = new Color[ledCount];
        }
        /// <summary>
        /// Sets the LED color at the position and applies color flips if needed
        /// </summary>
        /// <param name="index">Position of the LED</param>
        /// <param name="color">New color for the LED</param>
        public void SetLEDColor(int index, Color color) {
            if (index < 0 || index >= ledColors.Length) {
                Log.ForContext<BaseRenderer>().Debug("index: {index}, range [0, {LightCount}], ledColors length: {length}",
                                                                    index, LightCount - 1, ledColors.Length);
                throw new ArgumentOutOfRangeException("index");
            }
            ledColors[index] = color;
            colorsChanged = true;
        }
        /// <summary>
        /// Sets all the LEDs to the same color
        /// </summary>
        /// <param name="color">the new color</param>
        public void SetAllLEDColors(Color color) {
            for (int i = 0; i < ledColors.Length; i++) {
                SetLEDColor(i, color);
            }
        }

        public abstract void Start();
        public abstract void Stop();

        public virtual void Dispose() {
            // nothing to do
        }

        private static RendererHardwareInfo _hardwareInfo = new RendererHardwareInfo();

        /// <summary>
        /// Which hardware this renderer can be executed on
        /// </summary>
        public static RendererHardwareInfo GetHardwareInfo() => _hardwareInfo;

        /// <summary>
        /// Necessary hardware information
        /// <notes>Used by the setup process for helping the user setup the renderer</notes>
        /// </summary>
        public static Hardware_Type GetSupportedHardware() => Hardware_Type.All;
    }
}
