using System;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Models {
    public class AnimationModel : BaseModel {
        public bool Disabled { get; set; }
        public AnimationDataModel[] Animations;
        public AnimationState CurrentState { get; set; }
    }
    public class AnimationDataModel {
        public string Name { get; set; }
        public bool CurrentAnimation { get; set; }
        public ActiveAnimationProperty[] Properties { get; set; }
    }
}
