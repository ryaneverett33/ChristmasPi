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
                var hardware = ConfigurationManager.Instance.CurrentTreeConfig.hardware;
                switch (hardware.type) {
                    case RendererType.RPI_WS281x: {
                            if (WS281xRenderer == null)
                                WS281xRenderer = new WS281xRenderer(hardware.lightcount, hardware.datapin, ConfigurationManager.Instance.CurrentTreeConfig.hardware.fps);
                            return WS281xRenderer;
                        }
                    case RendererType.TEST_RENDER: {
                            if (testRenderer == null)
                                testRenderer = new TestRenderer(hardware.lightcount);
                            return testRenderer;
                        }
                    case RendererType.UNKNOWN:
                        throw new InvalidRendererException();
                }
            }
            return null;
        }
        public static bool TestRender(RendererType rendererType, int datapin) {
            bool success = false;
            lock (locker) {
                try {
                    switch (rendererType) {
                        case RendererType.RPI_WS281x: {
                            var renderer = new WS281xRenderer(1, datapin, 1);
                            renderer.Dispose();
                            success = true;
                            break;
                        }
                        default:
                            success = true;
                            break;
                    }
                }
                catch (Exception) {
                    success = false;
                }
            }
            return success;
        }
        public void Dispose() {
            if (!disposed) {
                if (WS281xRenderer != null)
                    WS281xRenderer.Dispose();
                if (testRenderer != null)
                    testRenderer.Dispose();
            }
        }
    }
}
