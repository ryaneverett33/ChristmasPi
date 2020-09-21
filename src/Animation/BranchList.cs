using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Interfaces;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public class BranchList {
        private Dictionary<int, BranchData> branches;

        public int Count => branches.Count;

        public BranchList(Branch[] branches) {
            if (branches == null)
                throw new ArgumentNullException("branches");
            this.branches = new Dictionary<int, BranchData>(branches.Length);
            for (int i = 0; i < branches.Length; i++) {
                this.branches[i] = new BranchData(branches[i], i);
            }
        }

        // get branch by friendly name
        public BranchData GetBranch(string name) {
            throw new NotImplementedException();
        }

        // get branch by index
        public BranchData GetBranch(int branchindex) {
            if (!branches.ContainsKey(branchindex))
                throw new ArgumentOutOfRangeException("branchindex");
            return branches[branchindex];
        }

        public void UpdateBranch(ref BranchData branch, int branchindex) {
            if (!branches.ContainsKey(branchindex))
                throw new ArgumentOutOfRangeException("branchindex");
            branches[branchindex] = branch;
        }

        // sets the friendly name of a branch to referenced later
        public void setFriendlyName(BranchData branch) {
            throw new NotImplementedException();
        }
    }
    public class BranchData {
        Branch branch;
        //string friendlyName;
        public int index;              // position in the tree
        public int LightCount => branch.LightCount;
        public FrameList list;

        public BranchData(Branch branch, int index) {
            this.branch = branch;
            //this.friendlyName = null;
            this.index = index;
            list = new FrameList();
        }

        public void Add(IAnimationFrame frame) {
            list.Add(frame);
        }
    }
}
