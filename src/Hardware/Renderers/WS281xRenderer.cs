using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Data;
using ChristmasPi.Data.Extensions;
using rpi_ws281x;

namespace ChristmasPi.Hardware.Renderers {
    /// <summary>
    /// A WS281x renderer based on the raspberry pi
    /// </summary>
    public class WS281xRenderer : BaseRenderer, IRenderer {
        private WS281x rpi;                             // WS281x object used for hardware access
        private RenderThread renderThread;              // thread used for rendering
        private object locker;
        public new bool AutoRender => true;

        /// <summary>
        /// Creates and starts a new WS281x renderer
        /// </summary>
        /// <param name="ledCount">Number of LEDs to render</param>
        /// <param name="pin">GPIO pin to connect to</param>
        public WS281xRenderer(int ledCount, int pin) {
            var settings = Settings.CreateDefaultSettings();
            settings.Channels[0] = new Channel(ledCount, pin);
            rpi = new WS281x(settings);
            ledColors = new List<Color>(ledCount);
            var defaultColor = ConfigurationManager.Instance.TreeConfiguration.tree.color.DefaultColor;
            for (int i = 0; i < ledCount; i++) {
                ledColors[i] = defaultColor;
            }
            locker = new object();
            renderThread = new RenderThread(this);
            base.LightCount = ledCount;
        }
        public override void Render(IRenderer obj) {
            WS281xRenderer renderer = (WS281xRenderer)obj;
            lock (renderer.locker) {
                // only render if the colors table has been updated
                if (renderer.colorsChanged) {
                    for (int i = 0; i < ledColors.Capacity; i++) {
                        rpi.SetLEDColor(0, i, ledColors[i]);
                    }
                    rpi.Render();
                    // reset flag
                    renderer.colorsChanged = false;
                }
            }
        }
        public override void Start() {
            renderThread.Start();
        }
        public override void Stop() {
            renderThread.Stop();
        }
    }
}
