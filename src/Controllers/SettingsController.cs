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
    public class SettingsController : ControllerBase
    {
        [HttpPost("update")]
        public IActionResult Update() {
            // /api/settings/update
            return new NotImplementedResult();
        }
        [HttpGet("info")]
        public IActionResult GetInfo() {
            // /api/settings/info
            return new NotImplementedResult();
        }
        [HttpPost("action/reload")]
        public IActionResult ActionReload() {
            // /api/settings/action/reload
            return new NotImplementedResult();
        }
        [HttpPost("action/reboot")]
        public IActionResult ActionReboot() {
            // /api/settings/action/reboot
            return new NotImplementedResult();
        }
        [HttpPost("action/confirm")]
        public IActionResult ActionConfirm() {
            // /api/settings/action/confirm
            return new NotImplementedResult();
        }
    }
}