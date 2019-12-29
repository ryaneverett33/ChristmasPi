using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Models;
using ChristmasPi.Data;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;

namespace ChristmasPi.Controllers {
    [ApiController]
    public class SetupController : Controller {
        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        [HttpPost("start")]
        public IActionResult Start() {
            // check if we can start the setup process
            if (!ConfigurationManager.Instance.CurrentTreeConfig.setup.firstrun)
                return new BadRequestObjectResult("Setup already ran");
            if (OperationManager.Instance.CurrentOperatingMode is ISetupMode)
                return new BadRequestObjectResult("Already started setup");
            // start setup mode
            OperationManager.Instance.SwitchModes("SetupMode");
            if (OperationManager.Instance.CurrentOperatingMode is ISetupMode)
                return new StatusCodeResult(500);
            return new OkResult();
        }
        [HttpGet("next")]
        public IActionResult Next() {
            return new TextResult("testy boi");
        }
    }
}