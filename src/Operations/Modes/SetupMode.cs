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
using ChristmasPi.Models;
using System.Drawing;
using System.IO;
using Serilog;
using Newtonsoft.Json;

namespace ChristmasPi.Operations.Modes {
    public class SetupMode : IOperationMode, ISetupMode {
        #region Properties
        public string Name => "SetupMode";
        public bool CanBeDefault => false;
        public bool IsInstallingAService => currentServiceInstaller != null;
        #endregion
        #region Fields
        private SetupStep[] steps;
        private Dictionary<string, string> validActions;
        public string CurrentStepName { get; private set; }
        public TreeConfiguration Configuration { get; private set; } 
        public bool IsSettingUpBranches { get; private set; }
        private IRenderer renderer;
        private List<Color> usedColors;
        private List<Tuple<Branch, Color>> branches;
        private int lightCount;
        private ServiceInstaller currentServiceInstaller;
        private bool installSchedulerService;
        private bool serviceHasUpdate;
        private bool servicesInstalled;
        private ServiceStatusModel lastStatusUpdate;
        private SetupProgress currentProgress;
        #endregion
        public SetupMode() {
            steps = new SetupStep[] {
                new SetupStep("start"),
                new SetupStep("hardware"),
                new SetupStep("lights"),
                new SetupStep("branches"),
                new SetupStep("defaults"),
                new SetupStep("services"),
                new SetupStep("finished")
            };
            validActions = new Dictionary<string, string>() {
                {"Index","index"},
                {"Start","start"},
                {"Next","next"},
                {"SetupHardware","hardware"},
                {"SetupLights","lights"},
                {"SetupBranches","branches"},
                {"SetupDefaults","defaults"},
                {"SetupServices","services"},
                {"Finished","finished"},
                {"SubmitHardware","hardware/submit"},
                {"SubmitLights","light/submit"},
                {"SubmitBranches","branches/submit"},
                {"SubmitDefaults","defaults/submit"},
                {"BranchesNewBranch","branch/new"},
                {"BranchesRemoveBranch","branch/remove"},
                {"BranchesAddLight","light/new"},
                {"BranchesRemoveLight","light/remove"},
                {"ServicesStartInstall","services/install"},
                {"ServicesGetProgress","services/progress"},
                {"ServicesFinish","services/finish"},
                {"SetupComplete","setup/complete"}
            };
            loadCurrentProgress();
            SetCurrentStep(null);
            Controllers.RedirectHandler.AddOnRegisteringLookupHandler(() => {
                if (!Controllers.RedirectHandler.IsActionLookupRegistered("Setup")) {
                    Controllers.RedirectHandler.RegisterActionLookup("Setup", validActions);
                }
                if (!Controllers.RedirectHandler.IsRuleFunctionRegistered("Setup")) {
                    Controllers.RedirectHandler.RegisterLookupRules("Setup", ShouldRedirect);
                }
            });
        }
        #region IOperationMode Methods
        public void Activate(bool defaultmode) {
            Log.ForContext("ClassName", "AnimationMode").Information("Activated Setup Mode");
            // TODO Load current setup progress
            Configuration = ConfigurationManager.Instance.CurrentTreeConfig;
            SetCurrentStep("start");
        }
        public void Deactivate() {
            Log.ForContext("ClassName", "AnimationMode").Information("Deactivated Setup Mode");
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
                if (steps[i].Name.Equals(currentpage, StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 >= steps.Length)
                        return null;
                    return steps[i+1].Name;
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
            Log.ForContext("ClassName", "AnimationMode").Debug("Setting current step: {newstep}", newstep);
            if (newstep == null || newstep.Length == 0 || newstep == "null")
                return;
            SetupStep step = steps.Where(step => step.Name.Equals(newstep)).Single();
            CurrentStepName = step.Name;
        }
        public void CompleteStep() {
            // completes the current step in a linear fashion
            if (CurrentStepName == null || CurrentStepName.Length == 0) {
                Log.ForContext("ClassName", "AnimationMode").Error("CompleteStep() CurrentStepName is null");
                return;
            }
            int currentStepIndex = -1;
            // get index of current step and check if previous steps are completed
            for (int i = 0; i < steps.Length; i++) {
                if (steps[i].Name == CurrentStepName) {
                    currentStepIndex = i;
                    break;
                }
                else {
                    if (!steps[i].Completed)
                        throw new ApplicationException("Setup not completed in linear order");
                }
            }
            if (currentStepIndex == -1)
                throw new ApplicationException("Unable to find index for the current setup step");
            steps[currentStepIndex].Completed = true;
        }
        public bool IsStepFinished(string step) {
            return steps.Where(s => s.Name == step).Single().Completed;
        }

        /// <summary>
        /// Complete the setup process and switch to the default operating mode
        /// </summary>
        public void Finish() {
            // set firstrun to false
            // set current configuration
            // save configuration
            Controllers.RedirectHandler.SetupComplete();
            Configuration.setup.firstrun = false;
            ConfigurationManager.Instance.CurrentTreeConfig = Configuration;
            ConfigurationManager.Instance.SaveConfiguration();
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

        public bool SetDefaults(string animation, string mode, string color) {
            if (animation == null || mode == null)
                return false;
            if (!Animation.AnimationManager.Instance.Animations.ContainsKey(animation))
                return false;
            string[] modes = OperationManager.Instance.GetDefaultableModes();
            if (!modes.Contains(mode))
                return false;
            Color defaultColor;
            try {
                defaultColor = Util.ColorConverter.Convert(color);
            }
            catch (Exception) {
                Log.ForContext("ClassName", "SetupMode").Error("SetDefaults() could not convert color: {color}", color);
                return false;
            }
            Configuration.tree.color.DefaultColor = defaultColor;
            Configuration.tree.defaultanimation = animation;
            Configuration.tree.defaultmode = mode;
            
            return true;
        }

        public void StartSettingUpBranches() {
            if (IsSettingUpBranches) {
                if (renderer != null)
                    renderer.Stop();
            }
            renderer = RenderFactory.GetRenderer(Configuration.hardware.type, Configuration);
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

        public bool StartServicesInstall(bool installSchedulerService) {
            // If the user hits reload instead of hitting continue
            if (servicesInstalled) {
                lastStatusUpdate = new ServiceStatusModel().AllDone();
                lastStatusUpdate.Output = "ChristmasPi.service already installed";
                if (installSchedulerService)
                    lastStatusUpdate.Output += "\nScheduler.service already installed";
                serviceHasUpdate = true;
                servicesInstalled = true;
                return true;
            }
            // If two different browsers attempt to install at the same time
            if (currentServiceInstaller != null)
                return false;
            this.installSchedulerService = installSchedulerService;
            Log.ForContext("ClassName", "AnimationMode").Debug("Install Scheduler Service? {installSchedulerService}", installSchedulerService);
            currentServiceInstaller = new ServiceInstaller("ChristmasPi.service", "some path");
            currentServiceInstaller.OnInstallFailure = serviceInstallHandler;
            currentServiceInstaller.OnInstallProgress = serviceInstallHandler;
            currentServiceInstaller.OnInstallSuccess = serviceInstallHandler;
            currentServiceInstaller.StartInstall();
            return true;
        }
        private void serviceInstallHandler(ServiceInstallState state) {
            switch (state.Status) {
                case InstallationStatus.Success:
                    // Wait until serviceHasUpdate has been toggled before installing next service
                    if (installSchedulerService) {
                        lastStatusUpdate = new ServiceStatusModel(currentServiceInstaller.GetWriter(), currentServiceInstaller.GetStatus());
                        serviceHasUpdate = true;
                        Task t = new Task(() => {
                            installSchedulerService = false;
                            while (serviceHasUpdate) {
                                // Slow Spin Lock
                                Task.Delay(25);
                            }
                            // Start install for Scheduler.service
                            currentServiceInstaller.Dispose();
                            currentServiceInstaller = new ServiceInstaller("Scheduler.service", "some path");
                            currentServiceInstaller.OnInstallFailure = serviceInstallHandler;
                            currentServiceInstaller.OnInstallProgress = serviceInstallHandler;
                            currentServiceInstaller.OnInstallSuccess = serviceInstallHandler;
                            currentServiceInstaller.StartInstall();
                        });
                        t.Start();
                    }
                    else {
                        lastStatusUpdate = lastStatusUpdate.AllDone();
                        serviceHasUpdate = true;
                        servicesInstalled = true;
                    }
                    break;
                case InstallationStatus.Failed:
                    // Clean up installer
                    lastStatusUpdate = new ServiceStatusModel(currentServiceInstaller.GetWriter(), currentServiceInstaller.GetStatus());
                    serviceHasUpdate = true;
                    currentServiceInstaller.Dispose();
                    currentServiceInstaller = null;
                    break;
                case InstallationStatus.Installing:
                    // Set service update info
                    lastStatusUpdate = new ServiceStatusModel(currentServiceInstaller.GetWriter(), currentServiceInstaller.GetStatus());
                    serviceHasUpdate = true;
                    break;
                default:
                    break;
            }
        }

        public ServiceStatusModel GetServicesInstallProgress() {
            if (currentServiceInstaller == null)
                return null;
            if (serviceHasUpdate) {
                serviceHasUpdate = false;
                return lastStatusUpdate;
            }
            else {
                return ServiceStatusModel.Stale();
            }
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

        private void loadCurrentProgress() {
            string SetupProgressFile = ConfigurationManager.Instance.RuntimeConfiguration.SetupProgressFile;
            if (SetupProgressFile != null) {
                if (!File.Exists(SetupProgressFile)) {
                    Log.ForContext("ClassName", "SetupMode").Information("Setup Progress file not found");
                    Log.ForContext("ClassName", "SetupMode").Debug("Using a blank setup progress object");
                    currentProgress = new SetupProgress();
                }
                else {
                    string json = "";
                    try {
                        json = File.ReadAllText(SetupProgressFile);
                        currentProgress = JsonConvert.DeserializeObject<SetupProgress>(json);
                    }
                    catch (JsonSerializationException jsonerr) {
                        Log.ForContext("ClassName", "SetupMode").Error(jsonerr, "Failed to deserialize progress file");
                        Log.ForContext("ClassName", "SetupMode").Debug("Progress file contents: {json}", json);   
                    }
                    catch (Exception e) {
                        Log.ForContext("ClassName", "SetupMode").Error(e, "Failed to load current progress");
                    }
                    finally {
                        if (currentProgress == null) {
                            Log.ForContext("ClassName", "SetupMode").Debug("Using a blank setup progress object");
                            currentProgress = new SetupProgress();
                        }
                    }
                }
            }
            else {
                Log.Debug("Using a blank setup progress object");
                currentProgress = new SetupProgress();
            }
        }
        #endregion
        public string ShouldRedirect(string controller, string action, string method) {
            Log.ForContext("ClassName", "AnimationMode").Debug("Called SetupMode ShouldRedirect");
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode)) {
                // If on the index page, don't redirect
                // ignore these actions to allow the ability to start setupmode
                if (action.ToLower() == "index" || action.ToLower() == "start" || action.ToLower() == "next")
                    return null;
                // Disallow access else
                return "/setup/";
            }
                
            // redirect to current page if setup has begun
            // redirect to setup start if setup hasn't begun
            bool activated = OperationManager.Instance.CurrentOperatingMode is ISetupMode;
            if (!activated) {
                if (action != "Index") // if not actively running setup, only allow the index page to be viewed
                    return "/setup/";
                else
                    return null;
            }
            else {
                // ignore these actions
                if (action.ToLower() == "start" || action.ToLower() == "next" || action.ToLower() == "finished")
                    return null;
                // redirect to current page
                // NOTE: SetCurrentPage is called after navigating to page, so redirect should account for going to the next page
                string nextPage = GetNext(CurrentStepName);
                Log.ForContext("ClassName", "AnimationMode").Debug("nextPage: {nextPage}, currentStepName: {CurrentStepName}", nextPage, CurrentStepName);
                if (method.ToUpper() == "POST") // don't redirect on POST requests
                    return null;
                if (action.ToUpper() == CurrentStepName.ToUpper())  // don't redirect on the current step
                    return null;
                if (action.ToUpper() == nextPage.ToUpper() && IsStepFinished(CurrentStepName)) // don't redirect when navigating to the next step
                    return null;
                if (action.ToLower() == "services/progress" && CurrentStepName.ToLower() == "services") // don't redirect when trying to get service installation progress
                    return null;
                return $"/setup/{CurrentStepName}";
            }
        }
    }
}