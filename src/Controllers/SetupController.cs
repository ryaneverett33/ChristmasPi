using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Models;
using ChristmasPi.Data;
using ChristmasPi.Hardware;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Data.Models;

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
            // redirect to current page if setup has already started
            if (OperationManager.Instance.CurrentOperatingMode is ISetupMode) {
                string currentStep = (OperationManager.Instance.CurrentOperatingMode as ISetupMode).CurrentStep;
                return new RedirectResult($"/setup/{currentStep}");
            }
            return View();
        }

        [HttpPost("/setup/start")]
        public IActionResult Start() {
            // check if we can start the setup process
            if (!ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun)
                return new BadRequestObjectResult("Setup already ran");
            if (OperationManager.Instance.CurrentOperatingMode is ISetupMode)
                return new BadRequestObjectResult("Already started setup");
            // start setup mode
            OperationManager.Instance.SwitchModes("SetupMode");
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new StatusCodeResult(500);
            return new OkResult();
        }
        [HttpGet("/setup/next")]
        public IActionResult Next(string current) {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
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
                case "finished":
                    return Finished();
                default:
                    return new BadRequestObjectResult($"Unable to find next step for {current}");
            }
        }
        [HttpGet("/setup/hardware")]
        public IActionResult SetupHardware() {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("hardware");
            var model = new SetupHardwareModel();
            return View("hardware", model);
        }
        [HttpGet("/setup/lights")]
        public IActionResult SetupLights() {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("lights");
            var model = new SetupLightsModel();
            return View("lights", model);
        }
        [HttpGet("/setup/branches")]
        public IActionResult SetupBranches() {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("branches");
            return View("branches");
        }
        [HttpGet("/setup/defaults")]
        public IActionResult SetupDefaults() {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
            (OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetCurrentStep("defaults");
            return View("defaults");
        }

        private IActionResult Finished() {
            // leave setupmode, redirect to home page
            return new RedirectResult("/");
        }
        [HttpPost("/setup/hardware/submit")]
        public IActionResult SubmitHardware() {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
            string pin = this.HttpContext.Request.Form["datapin"].ToString();
            string renderer = this.HttpContext.Request.Form["renderer"].ToString();
            object rendererType;        // This is actually a RendererType enum but TryParse can't out an Enum type (which is real dum)
            int datapin;
            if (!Enum.TryParse(typeof(RendererType), renderer, out rendererType))
                return View("hardware", new SetupHardwareModel("Invalid renderer format"));
            if (!int.TryParse(pin, out datapin))
                return View("hardware", new SetupHardwareModel("Invalid datapin format"));
            // save data
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetHardware((RendererType)rendererType, datapin)) {
                // Ignore invalid hardware settings while debugging on non-linux os
                
                //return View("hardware", new SetupHardwareModel("Invalid settings"));
                return new RedirectResult("/setup/next?current=hardware");
            }
            else
                return new RedirectResult("/setup/next?current=hardware");
        }
        [HttpPost("/setup/light/submit")]
        public IActionResult SubmitLights() {
            if (!(OperationManager.Instance.CurrentOperatingMode is ISetupMode))
                return new BadRequestObjectResult("Not in setup mode");
            string lightcountInput = this.HttpContext.Request.Form["lightcount"].ToString();
            string fpsInput = this.HttpContext.Request.Form["fps"].ToString();
            string brightnessInput = this.HttpContext.Request.Form["brightness"].ToString();
            int lightcount, fps, brightness;
            if (!int.TryParse(lightcountInput, out lightcount))
                return View("lights", new SetupLightsModel("Invalid lightcount format"));
            if (!int.TryParse(fpsInput, out fps))
                return View("lights", new SetupLightsModel("Invalid fps format"));
            if (!int.TryParse(brightnessInput, out brightness))
                return View("lights", new SetupLightsModel("Invalid brightness format"));
            if (!(OperationManager.Instance.CurrentOperatingMode as ISetupMode).SetLights(lightcount, fps, brightness))
                return View("lights", new SetupLightsModel("Invalid settings"));
            else
                return new RedirectResult("/setup/next?current=lights");
        }
        [HttpPost("/setup/branches/submit")]
        public IActionResult SubmitBranches() {
            return View("branches", new SetupBranchesModel("SubmitBranches not implemented yet"));
        }
    }
}