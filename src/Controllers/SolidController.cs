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

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolidController : ControllerBase
    {
        [HttpPost("update")]
        public IActionResult Update([FromBody]SolidUpdateArgument argument) {
            // /api/solid/update
            if (argument == null)
                return new BadRequestObjectResult("Argument is empty");
            if (argument.color == null)
                return new BadRequestObjectResult("Color argument is empty");
            if (OperationManager.Instance.CurrentOperatingModeName != "SolidColorMode")
                OperationManager.Instance.SwitchModes("SolidColorMode");
            Color newColor = ChristmasPi.Util.ColorConverter.Convert(argument.color);
            int result = (OperationManager.Instance.CurrentOperatingMode as ISolidColorMode).SetColor(newColor);
            return new StatusCodeResult(result);
        }
    }
    public class SolidUpdateArgument {
        public string color { get; set; }
    }
}