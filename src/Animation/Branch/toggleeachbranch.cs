using System;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.Branch {
    public class toggleeachbranch : IBranchAnimation, ILegacyAnimation {
        public string Name => "Toggle Each Branch (Legacy)";
        public bool isBranchAnimation => true;
        public bool isLegacyAnimation => true;
        public bool doStopExecuting { get; set; }
    }
}