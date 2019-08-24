using System;
using System.Threading;
using System.Threading.Tasks;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Data;

namespace ChristmasPi.Hardware.Renderers {
    public class RenderThread {
        private IRenderer renderer;         // The caller object 
        private Thread thread;              // worker thread
        private int waitTime;               // How long to wait (in ms) before rendering a new "frame"
        private int syncTime;               // Time left over from flooring the fps time
        private int fps;                    // How many "frames" are rendered per second
        private object locker;
        public bool Rendering { get; private set; } = false;
        public RenderThread(IRenderer renderer) {
            this.renderer = renderer;
            fps = ConfigurationManager.Instance.TreeConfiguration.hardware.fps;
            waitTime = calculateWaitTime();
            syncTime = Math.Clamp(1000 - (fps * waitTime), 0, 10);
            locker = new object();
            thread = new Thread(work);
        }
        public void Start() {
            lock (locker) {
                Rendering = true;
                thread.Start();
            }
        }
        public void Stop() {
            lock (locker) {
                Rendering = false;
            }
        }
        private void work() {
            while (Rendering) {
                for (int i = 0; i < fps; i++) {
                    try {
                        renderer.Render(renderer);
                    }
                    catch (Exception e) {
                        Console.WriteLine("LOGTHIS An exception occurred within the render thread.");
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                        Console.WriteLine("Render Thread exiting");
                        Rendering = false;
                        return;
                    }
                    lock (locker) {
                        Monitor.Wait(locker, waitTime);
                    }
                }
                if (syncTime != 0) {
                    lock (locker) {
                        Monitor.Wait(locker, syncTime);
                    }
                }
            }
        }
        private int calculateWaitTime() {
            if (fps <= 0 || fps > Constants.FPS_MAX) {
                fps = Constants.FPS_DEFAULT;
                Console.WriteLine("LOGTHIS Invalid fps value. Using default fps");
            }
            float value = (1f / (float)fps) * 1000f;
            return (int)Math.Floor(value);
        }
    }
}
