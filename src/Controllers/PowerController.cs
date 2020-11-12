using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Operations;
using ChristmasPi.Data;
using ChristmasPi.Operations.Interfaces;
using Serilog;

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase {
        [HttpPost("off")]
        public IActionResult Off() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // /api/power/off
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode) {
                (OperationManager.Instance.CurrentOperatingMode as IOffMode).TurnOff();
                Log.ForContext<PowerController>().Debug("Off(), turned off");
            }
            else {
                OperationManager.Instance.SwitchModes("OffMode");
                (OperationManager.Instance.CurrentOperatingMode as IOffMode).TurnOff();
                Log.ForContext<PowerController>().Debug("Off(), switched to OffMode and Turned off");
            }
            return Ok();
        }
        [HttpPost("on")]
        public IActionResult On() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // /api/power/on
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode) {
                // Switch to Default Operating Mode
                OperationManager.Instance.SwitchModes(ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode);
                Log.ForContext<PowerController>().Debug("On(), switched to {mode}", ConfigurationManager.Instance.CurrentTreeConfig.tree.defaultmode);
                return Ok();
            }
            else {
                // Handle silently for the scheduler
                Log.ForContext<PowerController>().Debug("On() was called but already turned on");
                return Ok();
            }
        }
        [HttpGet]
        public IActionResult Get() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            // /api/power/
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode)
                return new JsonResult(new { on = true });
            else
                return new JsonResult(new { on = false });
        }
    }
}