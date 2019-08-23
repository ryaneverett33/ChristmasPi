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
    public class SolidController : ControllerBase
    {
        [HttpPost("update")]
        public IActionResult Update() {
            // /api/solid/update
            return new NotImplementedResult();
        }
    }
}