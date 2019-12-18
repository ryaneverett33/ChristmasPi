using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Interfaces {
    public interface IAnimation : IAnimatable {
        int FPS { get; }
        int LightCount { get; }

        RenderFrame[] GetFrames(int fps, int lightcount);
    }
}