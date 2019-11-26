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

namespace ChristmasPi.Controllers {
    [Route("/")]
    public class HomeController : Controller {
        [HttpGet]
        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        [HttpGet("solid")]
        public IActionResult Solid() {
            return View();
        }

        [HttpGet("animation")]
        public IActionResult Animation() {
            var model = new AnimationModel {
                Disabled = (OperationManager.Instance.CurrentOperatingMode is IOffMode),
                Animations = AnimationManager.Instance.GetAnimations(),
                CurrentAnimation = ""
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
