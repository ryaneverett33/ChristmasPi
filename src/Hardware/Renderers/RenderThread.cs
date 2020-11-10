using System;
using System.Threading;
using System.Threading.Tasks;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Util;
using ChristmasPi.Data;
using Serilog;

namespace ChristmasPi.Hardware.Renderers {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Using boolean signature causes compile errors")]
    public class RenderThread : IDisposable {
        private IRenderer renderer;         // The caller object 
        private Thread thread;              // worker thread
        private int waitTime;               // How long to wait (in ms) before rendering a new "frame"
        private int syncTime;               // Time left over from flooring the fps time
        private int fps;                    // How many "frames" are rendered per second
        private CancellationTokenSource currentToken;
        private object locker;
        private bool disposed;

        /// <summary>
        /// Whether or not we are currently rendering
        /// </summary>
        public bool Rendering { get; private set; } = false;

        /// <summary>
        /// Whether the renderer will be rendering the last frame in the time allotment
        /// </summary>
        /// <example>When FPS = 30, LastFrame will be true when rendering the 30th frame, else false</example>
        public bool LastFrame { get; private set; } = false;

        public RenderThread(IRenderer renderer, int fps) {
            this.renderer = renderer;
            this.fps = fps;
            waitTime = ThreadHelpers.CalculateWaitTime(fps);
            syncTime = ThreadHelpers.CalculateSyncTime(fps, waitTime);
            locker = new object();
            thread = new Thread(work);
        }
        public void Start() {
            lock (locker) {
                Log.ForContext<RenderThread>().Debug("Starting render thread");
                Rendering = true;
                if (thread.ThreadState == ThreadState.Stopped) {
                    Log.ForContext<RenderThread>().Debug("Creating new worker thread");
                    thread = new Thread(work);
                }
                thread.Start();
            }
        }
        public void Stop() {
            lock (locker) {
                Log.ForContext<RenderThread>().Debug("Stopping render thread");
                Rendering = false;
                ThreadHelpers.WakeUpThread(currentToken);
            }
        }
        private void work() {
            bool doRender = false;
            lock (locker) {
                doRender = Rendering;
                currentToken = ThreadHelpers.RegisterWakeUp();
            }

            while (doRender) {
                for (int i = 0; i < fps; i++) {
                    if (i == fps - 1)
                        LastFrame = true;
                    else
                        LastFrame = false;
                    DateTime beforeRender = DateTime.Now;
                    try {
                        renderer.Render(renderer);
                    }
                    catch (Exception e) {
                        Log.ForContext<RenderThread>().Error(e, "Render Thread exiting");
                        Rendering = false;
                        return;
                    }
                    TimeSpan renderTime = DateTime.Now - beforeRender;
                    int newWaitTime = waitTime;
                    if (renderTime.TotalMilliseconds > waitTime) {
                        Log.ForContext<RenderThread>().Error("Took longer to render frame than fps waittime");
                        Log.ForContext<RenderThread>().Error("WaitTime: {waitTime}, renderTime: {renderTime}", waitTime, renderTime);
                    }
                    else {
                        newWaitTime = waitTime - (int)renderTime.TotalMilliseconds;
                    }
                    bool slept = ThreadHelpers.SafeSleep(currentToken, newWaitTime).Result;
                    if (!slept) {
                        Log.ForContext<RenderThread>().Debug("Sleep was cancelled, exiting thread");
                        // sleep was cancelled, exit thread
                        return;
                    }
                }
                if (syncTime != 0) {
                    ThreadHelpers.SafeSleep(currentToken, syncTime).Wait();
                }
                lock (locker) {
                    doRender = Rendering;
                }
            }
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
