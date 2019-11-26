using System;

namespace ChristmasPi.Models {
    public class AnimationModel {
        public bool Disabled { get; set; }
        public string[] Animations { get; set; }
        public string CurrentAnimation { get; set; }
        public PlayState CurrentState { get; set; }
    }
    public enum PlayState {
        Playing,
        Paused,
        Stopped
    }
}
