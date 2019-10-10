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
            try {
                IRenderer renderer = RenderFactory.GetRenderer();
                Color colorConverted = ChristmasPi.Util.ColorConverter.Convert(color);
                renderer.SetAllLEDColors(colorConverted);
                if (!renderer.AutoRender)
                    renderer.Render(renderer);
                return new OkResult();
            }
            catch (InvalidRendererException e) {
                Console.WriteLine("LOGTHIS SolidController::Update failed to get renderer");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            catch (InvalidColorFormatException) {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS SolidController::Update failed, an exception occured");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}