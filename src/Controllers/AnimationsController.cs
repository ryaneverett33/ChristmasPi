using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Util;

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimationsController : ControllerBase
    {
        [HttpPost("play")]
        public IActionResult PlayAnimation() {
            // /api/animations/play
            return new NotImplementedResult();
        }
        [HttpPost("pause")]
        public IActionResult PauseAnimation() {
            // /api/animations/pause
            return new NotImplementedResult();
        }
        [HttpPost("stop")]
        public IActionResult StopAnimation() {
            // /api/animations/stop
            return new NotImplementedResult();
        }
        [HttpGet()]
        public IActionResult GetAnimations() {
            // /api/animations
            return new NotImplementedResult();
        }
    }
}