using System;
using System.Threading;
using System.Threading.Tasks;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Util;
using ChristmasPi.Data;

namespace ChristmasPi.Hardware.Renderers {
    public class RenderThread : IDisposable {
        private IRenderer renderer;         // The caller object 
        private Thread thread;              // worker thread
        private int waitTime;               // How long to wait (in ms) before rendering a new "frame"
        private int syncTime;               // Time left over from flooring the fps time
        private int fps;                    // How many "frames" are rendered per second
        private CancellationTokenSource currentToken;
        private object locker;
        private bool disposed;
        public bool Rendering { get; private set; } = false;

        public RenderThread(IRenderer renderer, int fps) {
            this.renderer = renderer;
            this.fps = fps;
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
                ThreadHelpers.WakeUpThread(currentToken);
            }
        }
        private async void work() {
            bool doRender = false;
            lock (locker) {
                doRender = Rendering;
                currentToken = ThreadHelpers.RegisterWakeUp();
            }

            while (doRender) {
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
                    bool slept = await ThreadHelpers.SafeSleep(currentToken, waitTime);
                    if (!slept) {
                        // sleep was cancelled, exit thread
                        return;
                    }
                }
                if (syncTime != 0) {
                    await ThreadHelpers.SafeSleep(currentToken, syncTime);
                }
                lock (locker) {
                    doRender = Rendering;
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

        public void Dispose() {
            if (!disposed) {
                Stop();
                if (!thread.Join(500))
                    thread.Abort();
                renderer.Dispose();
                disposed = true;
            }
        }
    }
}
