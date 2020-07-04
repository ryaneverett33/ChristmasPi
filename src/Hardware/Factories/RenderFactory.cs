using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Hardware;
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

        /// <summary>
        /// Gets and configures the hardware renderer
        /// </summary>
        public static IRenderer GetRenderer(RendererType? rendererType = null, TreeConfiguration configuration = null) {
            if (rendererType.HasValue && configuration == null || configuration == null && !rendererType.HasValue)
                throw new InvalidOperationException("Either both params are null or filled, null arguments can not be mismatched");
            lock (locker) {
                RendererType type = rendererType.HasValue ? rendererType.Value : ConfigurationManager.Instance.CurrentTreeConfig.hardware.type;
                var hardware = configuration == null ? ConfigurationManager.Instance.CurrentTreeConfig.hardware : configuration.hardware;
                switch (type) {
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

        /// <summary>
        /// Tests a given renderer if it'll work on the current hardware
        /// </summary>
        /// <param name="rendererType">The renderer to test</param>
        /// <param name="datapin">Which data pin to initialize the renderer on</param>
        /// <returns>True if the renderer could be initialized, false if not</returns>
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
        
        /// <summary>
        /// Cleans up a previously initialized renderer
        /// </summary>
        /// <param name="rendererType">The renderer to clean up</param>
        public static void ReleaseRender(RendererType rendererType) {
            lock (locker) {
                switch (rendererType) {
                    case RendererType.RPI_WS281x: {
                            if (WS281xRenderer == null)
                                throw new Exception("Renderer not initialized yet, can't release");
                            WS281xRenderer.Dispose();
                            WS281xRenderer = null;
                            return;
                        }
                    case RendererType.TEST_RENDER: {
                            if (testRenderer == null)
                                throw new Exception("Renderer not initialized yet, can't release");
                            testRenderer.Dispose();
                            testRenderer = null;
                            return;
                        }
                    case RendererType.UNKNOWN:
                        throw new InvalidRendererException();
                }
            } 
        }
        /// <summary>
        /// Gets the hardware supported for a given renderer
        /// </summary>
        /// <param name="type">The renderer to look up info for</param>
        /// <returns>The hardware type supported</returns>
        /// <notes>Hardware_Type is a Flags enumeration and can represent multiple hardware types</notes>
        public static Hardware_Type GetSupportedHardwareForRenderer(RendererType type) {
            switch (type) {
                case RendererType.RPI_WS281x:
                    return WS281xRenderer.GetSupportedHardware();
                case RendererType.TEST_RENDER:
                    return TestRenderer.GetSupportedHardware();
                default:
                    throw new InvalidRendererException();
            }
        }
        /// <summary>
        /// Gets necessary setup information for a given renderer
        /// </summary>
        /// <param name="type">The renderer to look up info for</param>
        /// <returns>A RendererHardwareInfo object containing the placeholder, validation, and image strings</returns>
        public static RendererHardwareInfo GetRendererHardwareInfoForRenderer(RendererType type) {
            switch (type) {
                case RendererType.RPI_WS281x:
                    return WS281xRenderer.GetHardwareInfo();
                case RendererType.TEST_RENDER:
                    return TestRenderer.GetHardwareInfo();
                default:
                    throw new InvalidRendererException();
            }
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
