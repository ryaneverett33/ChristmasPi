using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data.Models;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.Interfaces {
    public interface IAnimation : IAnimatable {
        int FPS { get; }
        int LightCount { get; }

        RenderFrame[] GetFrames(int fps, int lightcount);

        void RegisterProperties();

        void AddProperties(AnimationProperty[] properties);

        bool RegisterProperty(Ref<object> reference, string name, object defaultValue);

        void ResolveProperties();
    }
}