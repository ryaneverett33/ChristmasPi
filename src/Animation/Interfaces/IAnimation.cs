using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data.Models;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.Interfaces {
    public interface IAnimation : IAnimatable {
        int FPS { get; }
        int LightCount { get; }

        AnimationProperty[] Properties { get; }

        RenderFrame[] GetFrames(int fps, int lightcount);

        void AddProperties(AnimationProperty[] properties);
    }
}