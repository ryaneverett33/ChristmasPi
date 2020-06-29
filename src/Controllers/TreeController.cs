using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Util;
using ChristmasPi.Operations;
using ChristmasPi.Data;

namespace ChristmasPi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TreeController : ControllerBase {
        [HttpGet("mode")]
        public IActionResult GetTreeMode() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            // /api/tree/mode
            return new JsonResult(OperationManager.Instance.CurrentOperatingInfo);
        }
        [HttpGet()]
        public IActionResult GetTreeInfo() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            // /api/tree
            var configuration = new {
                lightcount = ConfigurationManager.Instance.CurrentTreeConfig.hardware.lightcount,
                mode = OperationManager.Instance.CurrentOperatingModeName,
                name = ConfigurationManager.Instance.CurrentTreeConfig.tree.name
            };
            return new JsonResult(configuration);
        }
    }
}