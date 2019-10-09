namespace ChristmasPi.Animations.Interfaces {
    public interface IAnimation {
        string Name { get; }
        float TotalTime { get; }    // in s
        int TotalFrames { get; }

        IAnimationFrame[] GetFrames(int fps);
    }
}