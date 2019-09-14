using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Renderers;

namespace ChristmasPi.Hardware.Factories {
    public class RenderFactory {
        private static readonly object _locker = new object();
        private static object locker {
            get { return _locker; }
        }
        private static WS281xRenderer wS281XRenderer;
        private static TestRenderer testRenderer;

        public static IRenderer GetRenderer() {
            lock (locker) {
                var hardware = ConfigurationManager.Instance.TreeConfiguration.hardware;
                switch (hardware.type) {
                    case HardwareType.RPI_WS281x: {
                            if (wS281XRenderer == null)
                                wS281XRenderer = new WS281xRenderer(hardware.lightcount, hardware.datapin);
                            return wS281XRenderer;
                        }
                    case HardwareType.TEST_RENDER: {
                            if (testRenderer == null)
                                testRenderer = new TestRenderer();
                            return testRenderer;
                        }
                    case HardwareType.UNKNOWN:
                        throw new InvalidRendererException();
                }
            }
            return null;
        }
    }
}
