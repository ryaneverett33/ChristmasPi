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
using ChristmasPi.Data.Models;
using System.Drawing;

namespace ChristmasPi.Controllers {
    [Route("/")]
    public class HomeController : Controller {
        [HttpGet]
        public IActionResult Index() {
            //return View();
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode)
                return new RedirectResult("/power");
            else if (OperationManager.Instance.CurrentOperatingMode is IAnimationMode)
                return new RedirectResult("/animation");
            return new RedirectResult("/solid");
        }

        public IActionResult Privacy() {
            return View();
        }

        [HttpGet("solid")]
        public IActionResult Solid() {
            var model = new SolidModel {
                CurrentColor = (Color)OperationManager.Instance.GetProperty("SolidColorMode", "CurrentColor")
            };
            return View(model);
        }

        [HttpGet("animation")]
        public IActionResult Animation() {
            var model = new AnimationModel {
                Disabled = (OperationManager.Instance.CurrentOperatingMode is IOffMode),
                Animations = AnimationManager.Instance.GetAnimations(),
                CurrentAnimation = (string)OperationManager.Instance.GetProperty("AnimationMode", "CurrentAnimation"),
                CurrentState = (AnimationState)OperationManager.Instance.GetProperty("AnimationMode", "CurrentState")
            };
            return View(model);
        }
        [HttpGet("power")]
        public IActionResult Power() {
            var model = new PowerModel {
                PoweredOff = (OperationManager.Instance.CurrentOperatingMode is IOffMode)
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
