using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;

namespace ChristmasPi.Animation {
    public class Animator : IDisposable {
        private IAnimatable animation;
        private AnimationState _currentState;
        private IRenderer renderer;
        private bool useWorker;             // Whether or not to use a background worker or render events
        private Thread workerThread;        // only if renderer does not support AutoRender
        private Thread killerThread;        // only if running legacy animations
        private object locker;
        private int fps;
        private int lightcount;
        private int branchcount;
        private bool disposed;
        private AnimationFrame[] frames;

        public AnimationState CurrentState => _currentState;

        public Animator(IAnimatable animation, int fps, int lightcount, int branchcount) {
            this.animation = animation;
            this.fps = fps;
            this.lightcount = lightcount;
            this.branchcount = branchcount;
            disposed = false;
            locker = new object();
            frames = animation.isBranchAnimation ? null : (animation as IAnimation).GetFrames(fps, lightcount);
            _currentState = AnimationState.Stopped;
            renderer = RenderFactory.GetRenderer();
            if (renderer.AutoRender) {
                useWorker = false;
                // subscribe to events
                renderer.BeforeRenderEvent += OnBeforeRender;
                renderer.AfterRenderEvent += OnAfterRender;
            }
            else {
                useWorker = true;
                workerThread = new Thread(work);
                if (animation.isLegacyAnimation)
                    killerThread = new Thread(legacykiller);
            }
        }

        public void Start() {
            lock (locker) {
                _currentState = AnimationState.Animating;
                if (useWorker) {
                    workerThread.Start();
                    if (animation.isLegacyAnimation)
                        killerThread.Start();
                }
                renderer.Start();
            }
        }

        /// TODO implement
        public void Stop() {
            lock (locker) {
                _currentState = AnimationState.Stopped;
                if (useWorker) {
                    workerThread.Join(250);
                    killerThread.Join(250);
                }
                renderer.Stop();
            }
        }

        /// TODO implement
        public void Pause() {
            if (animation.isLegacyAnimation)
                throw new InvalidAnimationActionException("Cannot pause a legacy animation");
        }

        /// <summary>
        /// background worker thread
        /// </summary>
        private void work() {
            /// TODO implement
        }

        private void OnBeforeRender(object sender, RenderArgs args) {
            /// TODO implement
        }

        private void OnAfterRender(object sender, RenderArgs args) {
            /// TODO implement
        }

        /// <summary>
        /// Kills legacy animation
        /// </summary>
        private void legacykiller() {
            /// TODO implement
        }

        public void Dispose() {
            if (!disposed) {
                /// TODO implement
                disposed = true;
            }
        }
    }
}
