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
    public class ReactiveController : ControllerBase
    {
        [HttpPost("play")]
        public IActionResult Play() {
            // /api/reactive/play
            return new NotImplementedResult();
        }
        [HttpPost("pause")]
        public IActionResult Pause() {
            // /api/reactive/pause
            return new NotImplementedResult();
        }
        [HttpPost("stop")]
        public IActionResult Stop() {
            // /api/reactive/stop
            return new NotImplementedResult();
        }
        [HttpPost("update")]
        public IActionResult Update() {
            // /api/reactive/update
            return new NotImplementedResult();
        }
    }
}