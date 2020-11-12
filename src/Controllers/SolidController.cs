using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using ChristmasPi.Hardware.Factories;
using ChristmasPi.Hardware.Interfaces;
using ChristmasPi.Data.Exceptions;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using Serilog;

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolidController : ControllerBase
    {
        [HttpPost("update")]
        public IActionResult Update([FromBody]SolidUpdateArgument argument) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // /api/solid/update
            if (argument == null) {
                Log.ForContext<SolidController>().Debug("Update(), no arguments");
                return new BadRequestObjectResult("Argument is empty");
            }
            if (argument.color == null) {
                Log.ForContext<SolidController>().Debug("Update(), no color argument");
                return new BadRequestObjectResult("Color argument is empty");
            }
            if (OperationManager.Instance.CurrentOperatingModeName != "SolidColorMode")
                OperationManager.Instance.SwitchModes("SolidColorMode");
            Color newColor = Util.ColorConverter.Convert(argument.color);
            int result = (OperationManager.Instance.CurrentOperatingMode as ISolidColorMode).SetColor(newColor);
            Log.ForContext<SolidController>().Debug("Update(), returned {result} for color {color}", result, newColor.ToString());
            return new StatusCodeResult(result);
        }
    }
    public class SolidUpdateArgument {
        public string color { get; set; }
    }
}