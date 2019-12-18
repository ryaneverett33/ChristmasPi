using System;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Interfaces {
    public interface IAnimatable {
        bool isBranchAnimation { get; }
        string Name { get; }
        bool isDebugAnimation { get; }

        void RegisterProperties();
        void AddProperties(AnimationProperty[] properties);
        bool RegisterProperty(Ref<object> reference, string name, object defaultValue, PrimType type);
        void ResolveProperties();
        public string[] GetPropertyNames();
        ActiveAnimationProperty[] GetActiveAnimationProperties();
    }
}
