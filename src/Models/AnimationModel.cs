using System;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Models {
    public class AnimationModel {
        public bool Disabled { get; set; }
        public string[] Animations { get; set; }
        public string CurrentAnimation { get; set; }
        public AnimationState CurrentState { get; set; }
    }

}
