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
    public class RenderFactory : IDisposable {
        private static readonly object _locker = new object();
        private static object locker {
            get { return _locker; }
        }
        private static WS281xRenderer WS281xRenderer;
        private static TestRenderer testRenderer;
        private static bool disposed = false;

        public static IRenderer GetRenderer() {
            lock (locker) {
                var hardware = ConfigurationManager.Instance.TreeConfiguration.hardware;
                switch (hardware.type) {
                    case HardwareType.RPI_WS281x: {
                            if (WS281xRenderer == null)
                                WS281xRenderer = new WS281xRenderer(hardware.lightcount, hardware.datapin);
                            return WS281xRenderer;
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
        public void Dispose() {
            if (WS281xRenderer != null)
                WS281xRenderer.Dispose();
            if (testRenderer != null)
                testRenderer.Dispose();
        }
    }
}
