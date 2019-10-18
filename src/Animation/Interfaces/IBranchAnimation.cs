using ChristmasPi.Data.Models.Animation;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Animation.Interfaces {
    public interface IBranchAnimation : IAnimatable {
        public RenderFrame[] GetFrames(int fps);
        public void Init(Branch[] branches);
    }
}