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
            // redirect to current page if setup has already started
            //if (OperationManager.Instance.CurrentOperatingMode is ISetupMode) {
            //    string currentStep = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentStepName;
            //    return new RedirectResult($"/setup/{currentStep}");
            //}
            
            return View();
        }

        [HttpPost("/setup/start")]
        public IActionResult Start() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // check if we can start the setup process
            if (!ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun)
                return new BadRequestObjectResult("Setup already ran");
            if (OperationManager.Instance.CurrentOperatingMode is ISetupMode)
                return new BadRequestObjectResult("Already started setup");
            // start setup mode
            OperationManager.Instance.SwitchModes("SetupMode");
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new StatusCodeResult(500);
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CompleteStep();
            return new OkResult();
        }
        [HttpGet("/setup/next")]
        public IActionResult Next(string current) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            string next = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).GetNext(current);
            switch (next) {
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
                default:
                    return new BadRequestObjectResult($"Unable to find next step for {current}");
            }
        }
        [HttpGet("/setup/hardware")]
        public IActionResult SetupHardware() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("hardware");
            var model = new SetupHardwareModel();
            return View("hardware", model);
        }
        [HttpGet("/setup/lights")]
        public IActionResult SetupLights() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("lights");
            var model = new SetupLightsModel();
            return View("lights", model);
        }
        [HttpGet("/setup/branches")]
        public IActionResult SetupBranches() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            ISetupMode setupMode = (ISetupMode)OperationManager.Instance.CurrentOperatingMode;
            setupMode.SetCurrentStep("branches");
            setupMode.StartSettingUpBranches();
            var model = new SetupBranchesModel();
            return View("branches", model);
        }
        [HttpGet("/setup/defaults")]
        public IActionResult SetupDefaults() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("defaults");
            var model = new SetupDefaultsModel();
            return View("defaults", model);
        }
        [HttpGet("/setup/services")]
        public IActionResult SetupServices() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("services");
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
        [HttpGet("/setup/finished")]
        public IActionResult Finished() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("finished");
            return View();
        }
        [HttpPost("/setup/services/finish")]
        public IActionResult ServicesFinish() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CompleteStep();
            return new RedirectResult("/setup/next?current=services");
        }
        [HttpPost("/setup/hardware/submit")]
        public IActionResult SubmitHardware([FromForm]int datapin, [FromForm]string renderer) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // save data
            RendererType rendererType;
            if (!Enum.TryParse<RendererType>(renderer, out rendererType))
                return View("hardware", new SetupHardwareModel("Invalid renderer type"));
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetHardware(rendererType, datapin))
                return View("hardware", new SetupHardwareModel("Invalid settings"));
            else {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CompleteStep();
                return new RedirectResult("/setup/next?current=hardware");
            }
        }
        [HttpPost("/setup/light/submit")]
        public IActionResult SubmitLights([FromForm]int lightcount, [FromForm]int fps, [FromForm]int brightness) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetLights(lightcount, fps, brightness))
                return View("lights", new SetupLightsModel("Invalid settings"));
            else {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CompleteStep();
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
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CompleteStep();
                return Ok();
            }
            else
                return new BadRequestObjectResult("Invalid branch setup");
        }
        [HttpPost("/setup/defaults/submit")]
        public IActionResult SubmitDefaults([FromForm]string mode, [FromForm]string animation) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if ((OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetDefaults(animation, mode)) {
                (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CompleteStep();
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
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches)
                return new BadRequestObjectResult("Not in branch setup mode");
            Color? color = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).NewBranch();
            if (color == null)
                return new BadRequestObjectResult("Reached lightcount");
            var result = new {
                color = Util.ColorConverter.ToHex(color.Value)
            };
            return new JsonResult(result);
        }
        [HttpPost("/setup/branches/branch/remove")]
        public IActionResult BranchesRemoveBranch() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches)
                return new BadRequestObjectResult("Not in branch setup mode");
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).RemoveBranch())
                return new BadRequestObjectResult("Can't remove last branch");
            else
                return new OkResult();
        }
        [HttpPost("/setup/branches/light/new")]
        public IActionResult BranchesAddLight() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches)
                return new BadRequestObjectResult("Not in branch setup mode");
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).NewLight())
                return new BadRequestObjectResult("Reached lightcount");
            else
                return new OkResult();
        }
        [HttpPost("/setup/branches/light/remove")]
        public IActionResult BranchesRemoveLight() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).IsSettingUpBranches)
                return new BadRequestObjectResult("Not in branch setup mode");
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).RemoveLight())
                return new BadRequestObjectResult("Can't remove last light");
            else
                return new OkResult();
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
            return Ok();
        }
    }
    public class BranchesSubmitArgument {
        public Branch[] branches;
    }
    public class ServicesInstallArgument {
        public bool installScheduler;
    }
}