using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Operations;
using ChristmasPi.Data;
using ChristmasPi.Operations.Interfaces;

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
            }
            else {
                OperationManager.Instance.SwitchModes("OffMode");
                (OperationManager.Instance.CurrentOperatingMode as IOffMode).TurnOff();
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
                OperationManager.Instance.SwitchModes(OperationManager.Instance.DefaultOperatingMode);
                return Ok();
            }
            else {
                // Handle silently for the scheduler
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