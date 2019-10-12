using System;
using System.Linq;
using System.Collections.Generic;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Animation.Animations;
using ChristmasPi.Animation.Branch;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Animation {
    public class AnimationManager {
        #region Singleton Methods
        private static readonly AnimationManager _instance = new AnimationManager();
        public static AnimationManager Instance { get { return _instance; } }
        #endregion

        private Dictionary<string, IAnimation> animations;
        private Dictionary<string, IBranchAnimation> branchAnimations;

        private string _currentAnimation = null;
        private bool currentAnimationIsBranch = false;
        private bool _animating = false;
        private Animator animator;
    

        public string CurrentAnimation => _currentAnimation;
        public bool Animating => _animating;

        public void Init() {
            animations = new Dictionary<string, IAnimation>();
            branchAnimations = new Dictionary<string, IBranchAnimation>();
            string[] animationsClasses = getAnimationClasses();
            string[] branchAnimationClasses = getBranchAnimationClasses();
            foreach (string classname in animationsClasses) {
                /// TODO Handle if exception occurs when creating instance or casting
                IAnimation anim = (IAnimation)Activator.CreateInstance(Type.GetType(classname));
                animations.Add(anim.Name, anim);
            }
            foreach (string classname in branchAnimationClasses) {
                /// TODO Handle if exception occurs when creating instance or casting
                IBranchAnimation anim = (IBranchAnimation)Activator.CreateInstance(Type.GetType(classname));
                branchAnimations.Add(anim.Name, anim);
            }
            _animating = false;
            animator = null;
        }

        /// <summary>
        /// Gets a list of all animations in the executing assembly that implement the IAnimation interface
        /// </summary>
        /// <returns>List of animations to instantiate</returns>
        private string[] getAnimationClasses() {
            /// TODO use reflection to get classes
            return new string[] {
                typeof(flash).FullName
            };
        }

        /// <summary>
        /// Gets a list of all branch animations in the executing assembly that implement the IBranchAnimation interface
        /// </summary>
        /// <returns>List of branch animations to instantiate</returns>
        private string[] getBranchAnimationClasses() {
            /// TODO iuse reflection to get classes
            return new string[] {
                typeof(toggleeachbranch).FullName
            };
        }

        public string[] GetAnimations() {
            ICollection<string> keys = animations.Keys;
            return keys.ToArray<string>();
        }
        public string[] GetBranchAnimations() {
            ICollection<string> keys = branchAnimations.Keys;
            return keys.ToArray<string>();
        }

        public void StartAnimation(string animation, bool legacy = false) {
            /// TODO implement
        }
        public void PauseAnimation() {
            if (currentAnimationIsBranch && (branchAnimations[CurrentAnimation] as IAnimatable).isLegacyAnimation)
                throw new InvalidAnimationActionException("This legacy animation does not support pausing");
            /// TODO implement
        }
        public void StopAnimation() {
            /// TODO implement
        }

    }
}