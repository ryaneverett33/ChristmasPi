using System;
using System.Linq;
using System.Collections.Generic;
using ChristmasPi.Animations.Interfaces;
using ChristmasPi.Animations.Animations;
using ChristmasPi.Animations.Legacy;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Animations {
    public class AnimationManager {
        #region Singleton Methods
        private static readonly AnimationManager _instance = new AnimationManager();
        public static AnimationManager Instance { get { return _instance; } }
        #endregion

        private Dictionary<string, IAnimation> animations;
        private Dictionary<string, ILegacyAnimation> legacyAnimations;

        private string _currentAnimation = null;
        private bool currentAnimationIsLegacy = false;
        private bool _animating = false;
    

        public string CurrentAnimation => _currentAnimation;
        public bool Animating => _animating;

        public void Init() {
            animations = new Dictionary<string, IAnimation>();
            legacyAnimations = new Dictionary<string, ILegacyAnimation>();
            string[] animationsClasses = getAnimationClasses();
            string[] legacyAnimationsClasses = getLegacyAnimationClasses();
            foreach (string classname in animationsClasses) {
                /// TODO Handle if exception occurs when creating instance or casting
                IAnimation anim = (IAnimation)Activator.CreateInstance(Type.GetType(classname));
                animations.Add(anim.Name, anim);
            }
            foreach (string classname in legacyAnimationsClasses) {
                /// TODO Handle if exception occurs when creating instance or casting
                ILegacyAnimation anim = (ILegacyAnimation)Activator.CreateInstance(Type.GetType(classname));
                legacyAnimations.Add(anim.Name, anim);
            }
            _animating = false;
        }

        /// <summary>
        /// Gets a list of all animations in the executing assembly that implement the IAnimation interface
        /// </summary>
        /// <returns>List of animations</returns>
        private string[] getAnimationClasses() {
            return new string[] {
                typeof(flash).FullName
            };
        }

        /// <summary>
        /// Gets a list of all legacy animations in the executing assembly that implement the IAnimation interface
        /// </summary>
        /// <returns>List of legacy animations</returns>
        private string[] getLegacyAnimationClasses() {
            return new string[] {
                typeof(toggleeachbranch).FullName
            };
        }

        public string[] GetAnimations() {
            ICollection<string> keys = animations.Keys;
            return keys.ToArray<string>();
        }
        public string[] GetLegacyAnimations() {
            ICollection<string> keys = legacyAnimations.Keys;
            return keys.ToArray<string>();
        }

        public void StartAnimation(string animation, bool legacy = false) {

        }
        public void PauseAnimation() {
            if (currentAnimationIsLegacy)
                throw new InvalidAnimationActionException("Cannot pause a legacy animation");
        }
        public void StopAnimation() {

        }
    }
}