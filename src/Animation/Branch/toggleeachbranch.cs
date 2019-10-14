using System;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.Branch {
    public class toggleeachbranch : IBranchAnimation {
        public string Name => "Toggle Each Branch";
        public bool isBranchAnimation => true;
        public bool isLegacyAnimation => true;
    }
}