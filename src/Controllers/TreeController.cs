using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Util;

namespace ChristmasPi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TreeController : ControllerBase {
        [HttpGet("mode")]
        public IActionResult GetTreeMode() {
            // /api/tree/mode
            return new NotImplementedResult();
        }
        [HttpGet()]
        public IActionResult GetTreeInfo() {
            // /api/tree
            return new NotImplementedResult();
        }
    }
}