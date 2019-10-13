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
        public IActionResult Update(string color) {
            // /api/solid/update
            if (color == null)
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            if (OperationManager.Instance.CurrentOperatingModeName != "SolidColorMode")
                OperationManager.Instance.SwitchModes("SolidColorMode");
            Color newColor = ChristmasPi.Util.ColorConverter.Convert(color);
            int result = (OperationManager.Instance.CurrentOperatingMode as ISolidColorMode).SetColor(newColor);
            return new StatusCodeResult(result);
        }
    }
}