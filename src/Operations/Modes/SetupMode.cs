using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Util;
using System.Drawing;

namespace ChristmasPi.Operations.Modes {
    public class SetupMode : IOperationMode, ISetupMode {
        #region Properties
        public string Name => "SetupMode";
        public bool CanBeDefault => false;
        public bool IsInstallingAService => currentServiceInstaller != null;
        #endregion
        #region Fields
        private string[] steps;
        public string CurrentStep { get; private set; }
        public TreeConfiguration Configuration { get; private set; } 
        public bool IsSettingUpBranches { get; private set; }
        private IRenderer renderer;
        private List<Color> usedColors;
        private List<Tuple<Branch, Color>> branches;
        private int lightCount;
        private ServiceInstaller currentServiceInstaller;
        #endregion
        public SetupMode() {
            steps = new string[] {
                "start",
                "hardware",
                "lights",
                "branches",
                "defaults",
                "services",
                "finished"
            };
            SetCurrentStep(null);
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            Console.WriteLine("Activated Setup Mode");
            Configuration = ConfigurationManager.Instance.CurrentTreeConfig;
            SetCurrentStep("start");
        }
        public void Deactivate() {
            Console.WriteLine("Deactivated Setup Mode");
            SetCurrentStep("null");
        }
        public object Info() {
            return new {};
        }
        public object GetProperty(string property) {
            return null;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Gets the next step in the setup process
        /// </summary>
        /// <param name="currentpage">The current step in the setup process</param>
        /// <returns>The name of the next setup step</returns>
        public string GetNext(string currentpage) {
            for (int i = 0; i < steps.Length; i++) {
                if (steps[i].Equals(currentpage, StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 >= steps.Length)
                        return null;
                    return steps[i+1];
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the current step in the setup process
        /// </summary>
        /// <param name="newstep">The new setup step</param>
        /// <remarks>This should only be called by the SetupController</remarks>
        public void SetCurrentStep(string newstep) {
            CurrentStep = newstep;
        }

        /// <summary>
        /// Complete the setup process and switch to the default operating mode
        /// </summary>
        public void Finish() {
            // set firstrun to false
            // set current configuration
            // save configuration
        }

        /// <summary>
        /// Sets the hardware info
        /// </summary>
        /// <param name="rendererType">The type of renderer to use</param>
        /// <param name="datapin">The datapin to use</param>
        /// <returns>True if the hardware settings are valid or false if not valid</returns>
        public bool SetHardware(RendererType rendererType, int datapin) {
            // test if renderer settings are correct
            if (RenderFactory.TestRender(rendererType, datapin)) {
                Configuration.hardware.datapin = datapin;
                Configuration.hardware.type = rendererType;
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Sets the info for the lights
        /// </summary>
        /// <param name="lightcount">How many lights on the tree</param>
        /// <param name="fps">How quickly animations should be rendered</param>
        /// <param name="brightness">How bright the tree should be</param>
        /// <returns>True if the info is valid, false if parameters are incorrect</returns>
        public bool SetLights(int lightcount, int fps, int brightness) {
            // limit fps to 1-Constants.FPS_MAX and brightness to 0-255, 1 - lightcount to Constants.LIGHTS_MAX
            if (lightcount < 1)
                return false;
            if (fps < 1 || fps > Constants.FPS_MAX)
                return false;
            if (brightness < 0 || brightness > 255)
                return false;
            Configuration.hardware.brightness = brightness;
            Configuration.hardware.lightcount = lightcount;
            Configuration.hardware.fps = fps;
            return true;
        }

        public bool SetBranches(Branch[] branches) {
            if (branches == null)
                return false;
            if (branches.Any(b => {
                if (b.LightCount == 0 || b.start <= 0 || b.end <= 0)
                    return true;
                return false;
            }))
                return false;
            int lightCount = branches.Sum(b => b.LightCount);
            if (lightCount != Configuration.hardware.lightcount)
                return false;
            Configuration.tree.branches = new List<Branch>(branches);
            finishSettingUpBranches();
            return true;
        }

        public bool SetDefaults(string animation, string mode) {
            if (animation == null || mode == null)
                return false;
            if (!Animation.AnimationManager.Instance.Animations.ContainsKey(animation))
                return false;
            string[] modes = OperationManager.Instance.GetDefaultableModes();
            if (!modes.Contains(mode))
                return false;
            Configuration.tree.defaultanimation = animation;
            Configuration.tree.defaultmode = mode;
            return true;
        }

        public void StartSettingUpBranches() {
            if (IsSettingUpBranches) {
                if (renderer != null)
                    renderer.Stop();
            }
            renderer = RenderFactory.GetRenderer(Configuration);
            usedColors = new List<Color>();
            branches = new List<Tuple<Branch, Color>>();
            lightCount = 0;
            renderer.Start();
            IsSettingUpBranches = true;
            renderer.SetAllLEDColors(Constants.COLOR_OFF);
            if (!renderer.AutoRender)
                renderer.Render(renderer);
        }

        private void finishSettingUpBranches() {
            renderer.SetAllLEDColors(Constants.COLOR_OFF);
            if (!renderer.AutoRender)
                renderer.Render(renderer);
            branches = null;
            lightCount = 0;
            usedColors = null;
            renderer.Stop();
            IsSettingUpBranches = false;
            renderer.Dispose();
        }

        public Color? NewBranch() {
            if (lightCount >= Configuration.hardware.lightcount)
                return null;
            Color newColor = RandomColor.RandomColorNotInTable(usedColors);
            usedColors.Add(newColor);
            Branch branch;
            if (branches == null || branches.Count == 0)
                branch = new Branch() { start = 1, end = 1 };
            else {
                Tuple<Branch, Color> lastBranch = branches[branches.Count - 1];
                branch = new Branch() { start = lastBranch.Item1.end + 1, end = lastBranch.Item1.end + 1 };
            }
            branches.Add(new Tuple<Branch, Color>(branch, newColor));
            renderLight(newColor, true);
            return newColor;
        }

        public bool RemoveBranch() {
            if (branches.Count <= 1)
                return false;
            branches.RemoveAt(branches.Count - 1);
            lightCount = 0;
            foreach (Tuple<Branch, Color> branch in branches) {
                lightCount += branch.Item1.LightCount;
            }
            renderBranches();
            return true;
        }

        public bool NewLight() {
            if (lightCount >= Configuration.hardware.lightcount)
                return false;
            Color color = branches[branches.Count - 1].Item2;
            branches[branches.Count - 1].Item1.end++;
            renderLight(color, true);
            return true;
        }

        public bool RemoveLight() {
            Tuple<Branch, Color> branch = branches[branches.Count - 1];
            if (branch.Item1.end <= branch.Item1.start)
                return false;
            branches[branches.Count - 1].Item1.end--;
            renderLight(branch.Item2, false);
            return true;
        }

        public bool StartServicesInstall() {
            if (currentServiceInstaller != null)
                return false;
            currentServiceInstaller = new ServiceInstaller("some name", "some path");
            currentServiceInstaller.StartInstall();
            return true;
        }

        public InstallationProgress GetServicesInstallProgress() {
            if (currentServiceInstaller == null)
                return null;
            return currentServiceInstaller.GetProgress();
        }

        private void renderLight(Color color, bool increment) {
            if (increment)
                renderer.SetLEDColor(lightCount++, color);
            if (!increment)
                renderer.SetLEDColor(--lightCount, color);
            if (!renderer.AutoRender)
                renderer.Render(renderer);
            indicateLight(lightCount);
        }

        private void indicateLight(int light) {
            if (light >= Configuration.hardware.lightcount)
                return;
            renderer.SetLEDColor(light, Constants.INDICATION_COLOR);
            if (!renderer.AutoRender)
                renderer.Render(renderer);
        }

        private void renderBranches() {
            foreach (Tuple<Branch, Color> branch in branches) {
                for (int i = branch.Item1.start; i <= branch.Item1.end; i++) {
                    renderer.SetLEDColor(i, branch.Item2);
                }
            }
            if (!renderer.AutoRender)
                renderer.Render(renderer);
        }
        #endregion
    }
}