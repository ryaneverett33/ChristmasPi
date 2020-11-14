using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Models;
using ChristmasPi.Data;
using ChristmasPi.Hardware;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Data.Models;
using ChristmasPi.Util;
using System.Drawing;
using Serilog;

namespace ChristmasPi.Controllers {
/*
    Things to setup
Data pin
Lights
    lightcount
    additional
        brightness/inverted/color order
Branch setup
Defaults
    default color
    default mode
    default animation
*/
    [ApiController]
    public class SetupController : Controller {
        [HttpGet("/setup")]
        public IActionResult Index() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            return View();
        }

        [HttpPost("/setup/start")]
        public IActionResult Start() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // check if we can start the setup process
            if (!ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun)
                return new BadRequestObjectResult("Setup already ran");
            if (OperationManager.Instance.CurrentOperatingMode is ISetupMode) {
                if ((OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentStepName != "index" &&
                    (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentStepName != "start")
                    return new BadRequestObjectResult("Already started setup");
            }
            // start setup mode
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                OperationManager.Instance.SwitchModes("SetupMode");
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new StatusCodeResult(500);
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).Start();
            return new OkResult();
        }
        [HttpGet("/setup/next")]
        public IActionResult Next(string current) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            string next = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.GetNextStep(current);
            return handleRedirectForStepName(next);
        }
        [HttpGet("/setup/hardware")]
        public IActionResult SetupHardware() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;  
            var model = new SetupHardwareModel();
            return View("hardware", model);
        }
        [HttpGet("/setup/start")]
        public IActionResult start() {
            /*
                We don't have a view for this step, it's logically the index page so redirect there.
                Setup is started with a POST request so the GET request should be ignored.
            */
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;  
            return new RedirectResult("/setup/");
        }
        [HttpGet("/setup/lights")]
        public IActionResult SetupLights() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new SetupLightsModel();
            return View("lights", model);
        }
        [HttpGet("/setup/branches")]
        public IActionResult SetupBranches() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            try {
                ISetupMode setupMode = (ISetupMode)OperationManager.Instance.CurrentOperatingMode;
                Log.ForContext<SetupController>().Debug("SetupBranches(), setting up branches with {lightcount} lights", (OperationManager.Instance.CurrentOperatingMode as ISetupMode).Configuration.hardware.lightcount);
                setupMode.StartSettingUpBranches();
                var model = new SetupBranchesModel();
                return View("branches", model);
            }
            catch (Exception e) {
                Log.ForContext<SetupController>().Error(e, "SetupBranches(), an exception occurred");
                return new BadRequestObjectResult("An unknown error occurred");
            }
        }
        [HttpGet("/setup/defaults")]
        public IActionResult SetupDefaults() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new SetupDefaultsModel();
            return View("defaults", model);
        }
        [HttpGet("/setup/services")]
        public IActionResult SetupServices() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new SetupServicesModel();
            return View("services", model);
        }
        [HttpGet("/setup/services/progress")]
        public IActionResult ServicesGetProgress() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsInstallingAService)
                return new BadRequestObjectResult("No services are being installed");
            ServiceStatusModel status = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).GetServicesInstallProgress();
            if (status == null)
                return new BadRequestObjectResult("Unable to get progress");
            var model = new {
                Status = status.Status,
                Output = status.Output
            };
            return new JsonResult(model);
        }
        [HttpGet("/setup/aux/reboot")]
        public IActionResult ServicesAuxGetReboot() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.SetCurrentAuxStep("reboot");
            return View("reboot");
        }
        [HttpGet("/setup/finished")]
        public IActionResult Finished() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            return View();
        }
        [HttpPost("/setup/services/finish")]
        public IActionResult ServicesFinish() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
            return new RedirectResult("/setup/next?current=services");
        }
        [HttpPost("/setup/hardware/submit")]
        public IActionResult SubmitHardware([FromForm]string datapin, [FromForm]string renderer) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // save data
            RendererType rendererType;
            int datapinSafe;
            if (!int.TryParse(datapin, out datapinSafe))
                return View("hardware", new SetupHardwareModel("Invalid datapin, must be an integer"));
            if (!Enum.TryParse<RendererType>(renderer, out rendererType))
                return View("hardware", new SetupHardwareModel("Invalid renderer type"));
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetHardware(rendererType, datapinSafe))
                return View("hardware", new SetupHardwareModel("Invalid settings"));
            else {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
                return new RedirectResult("/setup/next?current=hardware");
            }
        }
        [HttpPost("/setup/lights/submit")]
        public IActionResult SubmitLights([FromForm]string lightcount, [FromForm]string fps, [FromForm]string brightness) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            int lightcountSafe;
            int fpsSafe;
            int brightnessSafe;
            if (!int.TryParse(lightcount, out lightcountSafe))
                return View("lights", new SetupLightsModel("Invalid lightcount, must be an integer"));
            if (!int.TryParse(fps, out fpsSafe))
                return View("lights", new SetupLightsModel("Invalid fps, must be an integer"));
            if (!int.TryParse(brightness, out brightnessSafe))
                return View("lights", new SetupLightsModel("Invalid brightness, must be an integer"));
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetLights(lightcountSafe, fpsSafe, brightnessSafe))
                return View("lights", new SetupLightsModel("Invalid settings"));
            else {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
                return new RedirectResult("/setup/next?current=lights");
            }
        }
        [HttpPost("/setup/branches/submit")]
        public IActionResult SubmitBranches([FromBody]Branch[] argument) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches)
                return new BadRequestObjectResult("Not in branch setup mode");
            if ((OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetBranches(argument)) {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
                return Ok();
            }
            else
                return new BadRequestObjectResult("Invalid branch setup");
        }
        [HttpPost("/setup/defaults/submit")]
        public IActionResult SubmitDefaults([FromForm]string mode, [FromForm]string animation, [FromForm]string color) {
            Log.ForContext<SetupController>().Information("Submitting new default color: {color}", color);
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if ((OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetDefaults(animation, mode, color)) {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
                return new RedirectResult("/setup/next?current=defaults");
            }
            else {
                var model = new SetupDefaultsModel("Invalid parameters");
                return View("defaults", model);
            }
        }

        [HttpPost("/setup/branches/branch/new")]
        public IActionResult BranchesNewBranch() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches) {
                Log.ForContext<SetupController>().Debug("BranchesNewBranch(), not in setup mode");
                return new BadRequestObjectResult("Not in branch setup mode");
            }   
            Color? color = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).NewBranch();
            if (color == null) {
                Log.ForContext<SetupController>().Debug("BranchesNewBranch(), setupmode failed to create new branch");
                return new BadRequestObjectResult("Reached lightcount");
            }
            var result = new {
                color = Util.ColorConverter.ToHex(color.Value)
            };
            Log.ForContext<SetupController>().Debug("BranchesNewBranch(), successfully created new branch with color {color}", result.color);
            return new JsonResult(result);
        }
        [HttpPost("/setup/branches/branch/remove")]
        public IActionResult BranchesRemoveBranch() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches) {
                Log.ForContext<SetupController>().Debug("BranchesRemoveBranch(), not in setup mode");
                return new BadRequestObjectResult("Not in branch setup mode");
            }
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).RemoveBranch()) {
                Log.ForContext<SetupController>().Debug("BranchesRemoveBranch(), failed to remove branch");
                return new BadRequestObjectResult("Can't remove last branch");
            }
            else {
                Log.ForContext<SetupController>().Debug("BranchesRemoveBranch(), successfully removed branch");
                return new OkResult();
            }
        }
        [HttpPost("/setup/branches/light/new")]
        public IActionResult BranchesAddLight() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches) {
                Log.ForContext<SetupController>().Debug("BranchesAddLight(), not in setup mode");
                return new BadRequestObjectResult("Not in branch setup mode");
            }
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).NewLight()) {
                Log.ForContext<SetupController>().Debug("BranchesAddLight(), failed to add light");
                return new BadRequestObjectResult("Reached lightcount");
            }
            else {
                //Log.ForContext<SetupController>().Debug("BranchesAddLight(), successfully added light");
                return new OkResult();
            }
        }
        [HttpPost("/setup/branches/light/remove")]
        public IActionResult BranchesRemoveLight() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches) {
                Log.ForContext<SetupController>().Debug("BranchesRemoveLight(), not in setup mode");
                return new BadRequestObjectResult("Not in branch setup mode");
            }
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).RemoveLight()) {
                Log.ForContext<SetupController>().Debug("BranchesRemoveLight(), failed to remove light");
                return new BadRequestObjectResult("Can't remove last light");
            }
            else {
                Log.ForContext<SetupController>().Debug("BranchesRemoveLight(), successfully removed light");
                return new OkResult();
            }
        }

        [HttpPost("/setup/services/install")]
        public IActionResult ServicesStartInstall([FromBody]ServicesInstallArgument installScheduler) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if ((OperationManager.Instance.CurrentOperatingMode as ISetupMode).StartServicesInstall(installScheduler.installScheduler))
                return Ok();
            else
                return new StatusCodeResult(500);
        }
        [HttpPost("/setup/complete")]
        public IActionResult SetupComplete() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).Finish();
            // switch to default operating mode
            OperationManager.Instance.SwitchModes(ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode);
            return Ok();
        }
        [HttpPost("/setup/aux/complete")]
        public IActionResult AuxCompleteStep() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            SetupProgress currentProgress = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress;
            if (currentProgress.CurrentStep.Contains("aux/")) {
                // Handle reboot case where frontend can exist with the wrong state. Only complete step if current step is an auxiliary function.
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentProgress.CompleteStep();
            }
            return handleRedirectForStepName(currentProgress.CurrentStep);
        }
        private IActionResult handleRedirectForStepName(string step) {
            switch (step) {
                case "hardware":
                    return new RedirectResult("/setup/hardware");
                case "lights":
                    return new RedirectResult("/setup/lights");
                case "branches":
                    return new RedirectResult("/setup/branches");
                case "defaults":
                    return new RedirectResult("/setup/defaults");
                case "services":
                    return new RedirectResult("/setup/services");
                case "finished":
                    return new RedirectResult("/setup/finished");
                case "aux/reboot":
                    return new RedirectResult("/setup/aux/reboot");
                default:
                    return new BadRequestObjectResult($"Unable to find next step for {step}");
            }
        }
    }
    public class BranchesSubmitArgument {
        public Branch[] branches;
    }
    public class ServicesInstallArgument {
        public bool installScheduler;
    }
}