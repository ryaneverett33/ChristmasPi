using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Data;
using rpi_ws281x;

namespace ChristmasPi.Hardware.Renderers {
    public class WS281xRenderer : IRenderer {
        private WS281x rpi;
        private RenderThread renderThread;
        private List<Color> ledColors;
        public WS281xRenderer(int ledCount, int pin) {
            var settings = Settings.CreateDefaultSettings();
            settings.Channels[0] = new Channel(ledCount, pin);
            rpi = new WS281x(settings);
            ledColors = new List<Color>(ledCount);
            var defaultColor = ConfigurationManager.Instance.TreeConfiguration.tree.color.DefaultColor;
            for (int i = 0; i < ledCount; i++) {
                ledColors[i] = defaultColor;
            }
            renderThread = new RenderThread(this);
            renderThread.Start();
        }
        public void Render(IRenderer obj) {
            throw new NotImplementedException();
        }

        public void SetLEDColor() {
            throw new NotImplementedException();
        }

    }
}
