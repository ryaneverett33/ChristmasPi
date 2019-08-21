using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChristmasServer.Animations {
    class AnimationManager {
        public bool isAlive = false;
        private Thread animThread;
        Dictionary<string, IAnimation> animationList = new Dictionary<string, IAnimation>(StringComparer.OrdinalIgnoreCase);
        bool isAnimating;
        IAnimation currentAnimation = null;
        Gpio gpio;

        public AnimationManager(Gpio gpio) {
            animationList.Add("Twinkle", new Twinkle(gpio));
            animationList.Add("Toggle Each Branch", new ToggleEachBranch(gpio));
            animationList.Add("Flash", new Flash(gpio));
            animationList.Add("Staircase", new Staircase(gpio));
            animationList.Add("Mirage", new Mirage(gpio));
            animationList.Add("Binary", new Binary(gpio));
            animationList.Add("Kels", new Kels(gpio));
            this.gpio = gpio;
            isAnimating = false;
            animThread = new Thread(() => playerThread());
            animThread.Start();
            isAlive = true;
        }
        //actually does the animation
        private void playerThread() {
            while (isAlive) {
                try {
                    if (isAnimating && currentAnimation != null) {
                        currentAnimation.playAnimation();
                    }
                    else {
                        Thread.Sleep(Timeout.Infinite);
                    }
                }
                catch (ThreadInterruptedException) {
                    //interrupts, then plays different animation
                }
                catch (ThreadAbortException) {
                    //cleanup
                }
            }
        }
        /// <summary>
        /// Pauses the currently playing animation
        /// </summary>
        /// <returns>True if there is an animation playing and it was paused</returns>
        /// <returns>False if there isn't an animation playing or the animation failed to pause</returns>
        /*public bool pauseAnimation() {
            if (isAnimating && !isPaused) {
                reason = new InterruptReason(InterruptAction.AnimationPause, null);
            }
            else {

            }
        }*/
        /// <summary>
        /// Plays a new animation regardless of whether an animation is currently playing
        /// Can also be used to restart the current animation
        /// </summary>
        /// <param name="animationName"></param>
        /// <returns>True if the animation exists and the animation started playing</returns>
        /// <returns>False if the animation doesn't exist or the animation failed to start</returns>
        public Tuple<bool,string> playAnimation(string animationName) {
            if (animationList.ContainsKey(animationName)) {
                animationList.TryGetValue(animationName, out currentAnimation);
                animThread.Interrupt();
                isAnimating = true;
                return new Tuple<bool, string>(true, null);
            }
            else {
                return new Tuple<bool, string>(false, null);
            }
        }
        /// <summary>
        /// Stops the current animation if it's playing
        /// </summary>
        /// <returns>True if there was an animation to stop and was successfully stopped</returns>
        /// <returns>False if there was no playing animation or the animation failed to stop</returns>
        public Tuple<bool, string> stopAnimation() {
            if (isAnimating) {
                currentAnimation = null;
                animThread.Interrupt();
                isAnimating = false;
                return new Tuple<bool, string>(true, null);
            }
            else {
                return new Tuple<bool, string>(false, null);
            }
        }
        public string[] getAnimationList() {
            List<string> keyList = new List<string>(animationList.Keys);
            string[] keys = keyList.ToArray();
            return keys;
        }
        /// <summary>
        /// Un-pauses the current playing animation
        /// </summary>
        /// <returns>True if there is a currently playing animation, that is paused, and it has successfully resumed</returns>
        /// <returns>False if there is no currently playing animation, or the currently playing animation isn't paused</returns>
        /*public bool resumeAnimation() {

        }*/
    }
    /*class InterruptReason {
        public InterruptAction action;
        public string animationName;
        public InterruptReason(InterruptAction act, string animationName) {
            this.action = act;
            this.animationName = animationName;
        }
    }
    enum InterruptAction {
        AnimationPause,
        AnimationResume,
        AnimationChange             //Includes Stop and play
    }*/
}
