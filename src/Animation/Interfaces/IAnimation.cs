using ChristmasPi.Data.Models;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.Interfaces {
    public interface IAnimation : IAnimatable {
        float TotalTime { get; }    // in s
        int TotalFrames { get; }
        int FPS { get; }
        int LightCount { get; }

        AnimationFrame[] GetFrames(int fps, int lightcount);
    }
}