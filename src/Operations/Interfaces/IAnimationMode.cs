namespace ChristmasPi.Operations.Interfaces {
    public interface IAnimationMode {
        int StartAnimation(string animationName);
        int PauseAnimation();
        int StopAnimation();
        string[] GetAnimations();
        int ResumeAnimation();
    }
}
