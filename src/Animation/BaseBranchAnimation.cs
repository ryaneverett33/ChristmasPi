using System;
using System.Collections.Generic;
using System.Linq;
using ChristmasPi.Animation.Interfaces;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Animation;

namespace ChristmasPi.Animation {
    public abstract class BaseBranchAnimation : IBranchAnimation {
        public virtual string Name { get { throw new NotImplementedException(); } }
        public virtual bool isDebugAnimation { get { return false; } }
        public bool isBranchAnimation => true;
        public int BranchCount => branchList.Count;
        public int FPS => fps;

        private int fps;
        BranchList branchList;
        public List<RenderFrame> frames;

        public BaseBranchAnimation() {
            frames = new List<RenderFrame>();
        }

        public void Init(Branch[] branches) {
            branchList = new BranchList(branches);
        }

        public virtual RenderFrame[] GetFrames(int fps) {
            if (frames.Count != 0 && this.fps == fps) {
                return frames.ToArray();
            }
            else {
                for (int i = 0; i < branchList.Count; i++) {
                    BranchData branch = branchList.GetBranch(i);
                    constructbranch(fps, ref branch);
                    branchList.UpdateBranch(ref branch, i);
                }
                frames.Clear();
                // assemble each animation into one
                RenderFrame[][] frameLists = getAnimations(fps);
                // keep a list of last used colors for reference when a branch is sleeping
                ColorList[] lastColors = new ColorList[branchList.Count];
                initLastColors(ref lastColors);
                // the total runtime of the animation is determined by the branch with the most frames
                // pad the other branches so that they match the runtime
                int totalFrames = frameLists.Max(list => list.Length);
                padFrames(ref frameLists, totalFrames);
                for (int i = 0; i < totalFrames; i++) {
                    // frameList.Length is the number of branches
                    // frameList[i].Length is the total number of frames
                    bool containsUpdate = frameLists.Any(list => {
                        return list[i].Action == FrameAction.Update;
                    });
                    if (!containsUpdate) {
                        frames.Add(new RenderFrame(FrameAction.Sleep));
                    }
                    else {
                        // join all renderframe arrays to one render frame array
                        // create a new list of colors to be used in the render frame
                        ColorList list = new ColorList();
                        for (int j = 0; j < frameLists.Length; j++) {
                            var frame = frameLists[j][i];           // get frame by branch, then by index of frame
                            if (frame.Action == FrameAction.Blank || frame.Action == FrameAction.Sleep) {
                                // if a branch is sleeping, use the last color rendered
                                ColorValue[] lastColorsFrame = lastColors[j].GetNonEvaluatedColors();
                                foreach (ColorValue lastColor in lastColorsFrame) {
                                    // branches are evaluated in order, so adding a color to the list automatically places it in the right location
                                    list.Add(lastColor);
                                }
                            }
                            else {
                                for (int x = 0; x < frame.Colors.Count; x++) {
                                    list.Add(frame.Colors[x]);
                                    // save color in the lastcolor array to use when the animation is sleeping
                                    lastColors[j].SetColor(x, frame.Colors[x]);
                                }
                            }
                        }
                        // create a new frame from the color list and add it to the total collection of frames
                        frames.Add(new RenderFrame(list));
                    }
                }
                return frames.ToArray();
            }
        }

        public virtual void constructbranch(int fps, ref BranchData branch) {
            this.fps = fps;
        }

        /// <summary>
        /// Gets each branch animation and formats them into an array of non-evaluated renderframes
        /// </summary>
        /// <param name="fps">The desired frames per second</param>
        /// <returns>An array of each branch's animation frames</returns>
        /// <remarks>[0][1] returns branch 0's second render frame</remarks>
        private RenderFrame[][] getAnimations(int fps) {
            List<RenderFrame[]> lists = new List<RenderFrame[]>(branchList.Count);
            for (int i = 0; i < branchList.Count; i++) {
                lists.Add(branchList.GetBranch(i).list.ToFrames(fps));
            }
            return lists.ToArray();
        }

        /// <summary>
        /// Pads the length of the render frame arrays so that all arrays are the same length
        /// </summary>
        /// <param name="frames">The RenderFrame array with arrays that need to be padded</param>
        /// <param name="totalFrames">The total number of the frames the renderframe should be</param>
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
        
        /// <summary>
        /// Initializes the list of lastColors
        /// </summary>
        /// <param name="arr">Array of colorlist objects to initialize</param>
        private void initLastColors(ref ColorList[] arr) {
            for (int i = 0; i < arr.Length; i++) {
                BranchData data = branchList.GetBranch(i);
                if (arr[i] == null) {
                    arr[i] = new ColorList();
                }
                for (int j = 0; j < data.LightCount; j++) {
                    arr[i].Add(Constants.COLOR_OFF);
                }
            }
        }
    }
}
