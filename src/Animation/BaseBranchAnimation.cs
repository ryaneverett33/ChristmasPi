using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public abstract class BaseBranchAnimation : IBranchAnimation {
        public virtual string Name { get { throw new NotImplementedException(); } }
        public bool isBranchAnimation => true;
        public int BranchCount => branchList.Count;
        public int FPS => fps;

        private int fps;
        BranchList branchList;
        public List<RenderFrame> frames;

        public BaseBranchAnimation(Branch[] branches) {
            branchList = new BranchList(branches);
            frames = new List<RenderFrame>();
        }

        public virtual RenderFrame[] GetFrames(int fps) {
            throw new NotImplementedException();
            if (frames.Count != 0 && this.fps == fps) {
                return frames.ToArray();
            }
            else {
                for (int i = 0; i < branchList.Count; i++) {
                    BranchData branch = branchList.GetBranch(i);
                    constructbranch(fps, ref branch);
                    branchList.UpdateBranch(ref branch, i);
                }
                // assemble each animation into one
                RenderFrame[][] frameLists = getAnimations(fps);
                int totalFrames = frameLists.Max(list => list.Length);
                padFrames(ref frameLists, totalFrames);
                for (int i = 0; i < totalFrames; i++) {
                    bool containsUpdate = frameLists.Any(list => {
                        return list[i].Action == FrameAction.Update;
                    });
                    if (!containsUpdate)
                        frames.Add(new RenderFrame(FrameAction.Sleep));
                    else {
                        // join all renderframe arrays to one render frame array
                        for (int j = 0; j < frameLists[i].Length; j++) {
                            
                        }
                    }
                }
            }
        }

        public virtual void constructbranch(int fps, ref BranchData branch) {
            this.fps = fps;
        }

        // get the render frame array for each animation
        private RenderFrame[][] getAnimations(int fps) {
            List<RenderFrame[]> lists = new List<RenderFrame[]>(branchList.Count);
            for (int i = 0; i < branchList.Count; i++) {
                lists[i] = branchList.GetBranch(i).list.ToFrames(fps);
            }
            return lists.ToArray();
        }

        // make sure each RenderFrame array is the same length
        private void padFrames(ref RenderFrame[][] frames, int totalFrames) {
            for (int i = 0; i < frames.Length; i++) {
                RenderFrame[] frame = frames[i];
                if (frame.Length == totalFrames)
                    continue;
                RenderFrame[] newFrames = new RenderFrame[totalFrames];
                for (int j = 0; j < totalFrames; j++) {
                    if (j < frame.Length)
                        newFrames[j] = frame[j];
                    else
                        newFrames[j] = new RenderFrame(FrameAction.Blank);
                }
                frames[i] = newFrames;
            }
        }
    }
}
