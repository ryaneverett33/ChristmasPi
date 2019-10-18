using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Animation;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data.Models;
using ChristmasPi.Data;
using Microsoft.AspNetCore.Http;

namespace ChristmasPi.Operations.Modes {
    public class AnimationMode : IOperationMode, IAnimationMode {
        #region Properties
        public string Name => "AnimationMode";
        #endregion

        #region Fields
        AnimationManager animationManager;                      // Quick reference to the static animation manager instance
        private Animator animator;                              // The current animator object (if an animation is playing)
        // If the current animation is a branch animation
        private bool currentAnimationIsBranch => (animator != null ? animator.CurrentAnimation.isBranchAnimation : false);                  

        public string CurrentAnimation => (animator != null ? animator.CurrentAnimation.Name : "None playing");
        public bool Animating => animator.CurrentState == AnimationState.Animating;
        public bool Paused => animator.CurrentState == AnimationState.Paused;
        #endregion
        public AnimationMode() {
            animationManager = AnimationManager.Instance;
            animator = null;
        }
        #region IOperationMode Methods
        public void Activate() {
            Console.WriteLine("Activated Animation Mode");
        }
        public void Deactivate() {
            if (Animating)
                StopAnimation();
            Console.WriteLine("Deactivated Animation Mode");
        }
        public object Info() {
            /// TODO fill in with relevant info
            return new {
                animating = Animating,
                paused = Paused,
                currentanimation = CurrentAnimation
            };
        }
        #endregion
        #region Methods
        public int StartAnimation(string animationName) {
            if (animator != null && (animator.CurrentState == AnimationState.Animating ||
                animator.CurrentState == AnimationState.Paused))
                return StatusCodes.Status405MethodNotAllowed;
            if (!animationManager.Animations.ContainsKey(animationName))
                return StatusCodes.Status400BadRequest;
            IAnimatable animation = animationManager.Animations[animationName];
            animator = new Animator(animation, ConfigurationManager.Instance.CurrentTreeConfig.hardware.fps,
                ConfigurationManager.Instance.CurrentTreeConfig.hardware.lightcount,
                0);
            try {
                animator.Start();
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status500InternalServerError;
            }
        }

        public int PauseAnimation() {
            if (animator == null || animator.CurrentState == Data.Models.AnimationState.Stopped ||
                animator.CurrentState == AnimationState.Error)
                return StatusCodes.Status400BadRequest;
            try {
                animator.Pause();
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status500InternalServerError;
            }
        }

        public int StopAnimation() {
            if (animator == null || animator.CurrentState == Data.Models.AnimationState.Error)
                return StatusCodes.Status405MethodNotAllowed;
            try {
                animator.Stop();
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status500InternalServerError;
            }
        }

        public int ResumeAnimation() {
            if (animator.CurrentState != AnimationState.Paused)
                return StatusCodes.Status405MethodNotAllowed;
            try {
                animator.Resume();
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS AnimationManager::StartAnimation() encountered an exception while stopping animation");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return StatusCodes.Status500InternalServerError;
            }
        }

        public string[] GetAnimations() {
            return animationManager.GetAnimations();
        }
        #endregion


    }
}
