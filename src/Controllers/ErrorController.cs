using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Models;
using ChristmasPi.Operations;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Animation;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using System.Drawing;

namespace ChristmasPi.Controllers {
    [Route("error")]
    public class ErrorController : Controller {
        
        [HttpGet("notadmin")]
        public IActionResult NotAdmin() {
            return View();
        }
    }
}
