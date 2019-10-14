using System;
using ChristmasPi.Data;
using ChristmasPi.Animation.Interfaces;

namespace ChristmasPi.Animation.Branch {
    public class toggleeachbranch : IBranchAnimation {
        public string Name => "Toggle Each Branch";
        public bool isBranchAnimation => true;
        private Data.Models.Branch[] branches;

        public toggleeachbranch() {
            branches = ConfigurationManager.Instance.CurrentTreeConfig.tree.branches.ToArray();
        }
    }
}