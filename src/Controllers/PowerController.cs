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
            // /api/power/on
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode) {
                // Switch to Default Operating Mode
                OperationManager.Instance.SwitchModes(OperationManager.Instance.DefaultOperatingMode);
                return Ok();
            }
            else {
                return BadRequest("Tree is already on, can't turn on");
            }
        }
        [HttpGet]
        public IActionResult Get() {
            // /api/power/
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode)
                return new JsonResult(new { on = true });
            else
                return new JsonResult(new { on = false });
        }
    }
}