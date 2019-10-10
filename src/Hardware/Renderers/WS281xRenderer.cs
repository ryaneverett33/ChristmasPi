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
        public new bool AutoRender => true;
        private WS281x rpi;                             // WS281x object used for hardware access
        private RenderThread renderThread;              // thread used for rendering
        private object locker;
        private bool disposed = false;


        /// <summary>
        /// Creates and starts a new WS281x renderer
        /// </summary>
        /// <param name="ledCount">Number of LEDs to render</param>
        /// <param name="pin">GPIO pin to connect to</param>
        public WS281xRenderer(int ledCount, int pin) {
            var settings = Settings.CreateDefaultSettings();
            settings.Channel_1 = new Channel(ledCount, 
                                            pin, 
                                            (byte)ConfigurationManager.Instance.TreeConfiguration.hardware.brightness, 
                                            ConfigurationManager.Instance.TreeConfiguration.hardware.invert, 
                                            getStripTypeFromColorOrder(ConfigurationManager.Instance.TreeConfiguration.tree.color.colororder));
            rpi = new WS281x(settings);
            base.ledColors = new List<Color>(ledCount);
            base.InitList();
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

        public override void Dispose() {
            if (disposed)
                return;
            renderThread.Stop();
            rpi.Dispose();
            base.Dispose();
            disposed = true;
        }

        /// <summary>
        /// Converts color order from a string to it's ws281x byte order representation
        /// </summary>
        /// <param name="order">The color order parameter</param>
        /// <returns>A WS2811x StripType</returns>
        private StripType getStripTypeFromColorOrder(string order) {
            switch (order.ToUpper().Trim()) {
                case "RGB":
                    return StripType.WS2811_STRIP_RGB;
                case "RBG":
                    return StripType.WS2811_STRIP_RBG;
                case "GRB":
                    return StripType.WS2811_STRIP_GRB;
                case "GBR":
                    return StripType.WS2811_STRIP_GBR;
                case "BRG":
                    return StripType.WS2811_STRIP_BRG;
                case "BGR":
                    return StripType.WS2811_STRIP_BGR;
                default:
                    return StripType.WS2811_STRIP_RGB;
            }
        }
    }
}
