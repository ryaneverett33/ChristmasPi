using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Models;

namespace ChristmasPi.Controllers {
    public class SetupController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}