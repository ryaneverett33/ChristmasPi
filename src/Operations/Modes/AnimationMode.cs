using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Operations.Utils;
using ChristmasPi.Animation;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data.Models;
using ChristmasPi.Data;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ChristmasPi.Operations.Modes {
    public class AnimationMode : IOperationMode, IAnimationMode {
        #region Properties
        public string Name => "AnimationMode";
        public bool CanBeDefault => true;
        public AnimationState CurrentState => animator == null ? AnimationState.Stopped : animator.CurrentState;
        public string CurrentAnimation => animator != null ? animator.CurrentAnimation.Name : "None playing";
        public bool Animating => animator != null ? animator.CurrentState == AnimationState.Animating : false;
        public bool Paused => animator != null ? animator.CurrentState == AnimationState.Paused : false;
        #endregion

        #region Fields
        AnimationManager animationManager;                      // Quick reference to the static animation manager instance
        private Animator animator;                              // The current animator object (if an animation is playing)
        // If the current animation is a branch animation
        private bool currentAnimationIsBranch => (animator != null ? animator.CurrentAnimation.isBranchAnimation : false);                  
        #endregion
        public AnimationMode() {
            animationManager = AnimationManager.Instance;
            animator = null;
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            Log.ForContext<AnimationMode>().Information("Activated Animation Mode");
            if (defaultmode) {
                // try and play default animation
                if (StartAnimation(ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultanimation) != 200) {
                    Log.ForContext<AnimationMode>().Error("Failed to play default animation");
                    StartAnimation(Constants.DEFAULT_ANIMATION);
                }
            }
        }
        public void Deactivate() {
            if (Animating)
                StopAnimation();
            Log.ForContext<AnimationMode>().Information("Deactivated Animation Mode");
        }
        public object Info() {
            return new {
                animating = Animating,
                paused = Paused,
                currentanimation = CurrentAnimation
            };
        }
        #endregion
        #region Methods
        public int StartAnimation(string animationName) {
            if (!animationManager.Animations.ContainsKey(animationName))
                return StatusCodes.Status400BadRequest;
            if (animator != null && (animator.CurrentState == AnimationState.Animating ||
                animator.CurrentState == AnimationState.Paused)) {
                // stop currently playing animation
                StopAnimation();
            }
            IAnimatable animation = animationManager.Animations[animationName];
            animator = new Animator(animation, ConfigurationManager.Instance.CurrentTreeConfig.hardware.fps,
                ConfigurationManager.Instance.CurrentTreeConfig.hardware.lightcount,
                0);
            try {
                animator.Start();
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an invalid animation action exception while stopping animation");
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an exception while stopping animation");
                return StatusCodes.Status500InternalServerError;
            }
        }
        public object GetProperty(string property) {
            return PropertyHelper.ResolveProperty(property, this, typeof(AnimationMode));
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
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an invalid animation action exception while stopping animation");
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an exception while stopping animation");
                return StatusCodes.Status500InternalServerError;
            }
        }

        public int StopAnimation() {
            if (animator == null || animator.CurrentState == Data.Models.AnimationState.Error)
                return StatusCodes.Status405MethodNotAllowed;
            try {
                animator.Stop();
                animator = null;
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an invalid animation action exception while stopping animation");
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an exception while stopping animation");
                return StatusCodes.Status500InternalServerError;
            }
        }

        public int ResumeAnimation() {
            if (animator == null || animator.CurrentState != AnimationState.Paused)
                return StatusCodes.Status405MethodNotAllowed;
            try {
                animator.Resume();
                return StatusCodes.Status200OK;
            }
            catch (InvalidAnimationActionException e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an invalid animation action exception while stopping animation");
                return StatusCodes.Status400BadRequest;
            }
            catch (Exception e) {
                Log.ForContext<AnimationMode>().Error(e, "StartAnimation() encountered an exception while stopping animation");
                return StatusCodes.Status500InternalServerError;
            }
        }

        public string[] GetAnimations() {
            return animationManager.GetAnimations();
        }
        #endregion


    }
}
