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
        private static object locker {
            get {
                if (locker == null) {
                    locker = new object();
                }
                return locker;
            }
            set { locker = value; }
        }
        private static WS281xRenderer wS281XRenderer;

        public IRenderer GetRenderer() {
            lock (locker) {
                var hardware = ConfigurationManager.Instance.TreeConfiguration.hardware;
                switch (hardware.type) {
                    case HardwareType.RPI_WS281x: {
                            if (wS281XRenderer == null)
                                wS281XRenderer = new WS281xRenderer(hardware.lightcount, hardware.datapin);
                            return wS281XRenderer;
                        }
                    case HardwareType.UNKNOWN:
                        throw new InvalidRendererException();
                }
            }
            return null;
        }
    }
}
