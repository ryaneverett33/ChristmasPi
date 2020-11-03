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
    [Route("/")]
    public class HomeController : Controller {
        public HomeController() {
            RedirectHandler.AddOnRegisteringLookupHandler(() => {
                RedirectHandler.RegisterActionLookup("Home", new Dictionary<string, string>() {
                    {"Index", "index"},
                    {"Solid", "solid"},
                    {"Animation", "animation"},
                    {"Power", "power"},
                    {"Schedule", "schedule"}
                });
            });
        }
        [HttpGet]
        public IActionResult Index() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            if (OperationManager.Instance.CurrentOperatingMode is IOffMode)
                return new RedirectResult("/power");
            else if (OperationManager.Instance.CurrentOperatingMode is IAnimationMode)
                return new RedirectResult("/animation");
            return new RedirectResult("/solid");
        }

        [HttpGet("solid")]
        public IActionResult Solid() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new SolidModel {
                CurrentColor = (Color)OperationManager.Instance.GetProperty("SolidColorMode", "CurrentColor"),
                DefaultColor = ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor
            };
            return View(model);
        }

        [HttpGet("animation")]
        public IActionResult Animation() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            string CurrentAnimation = (string)OperationManager.Instance.GetProperty("AnimationMode", "CurrentAnimation");
            List<AnimationDataModel> dataModels = new List<AnimationDataModel>();
            foreach (string animation in AnimationManager.Instance.GetAnimations()) {
                dataModels.Add(new AnimationDataModel {
                    Name = animation,
                    CurrentAnimation = animation.Equals(CurrentAnimation, StringComparison.CurrentCulture),
                    Properties = AnimationManager.Instance.GetAnimationProperties(animation)
                });
            }
            var model = new AnimationModel {
                Disabled = (OperationManager.Instance.CurrentOperatingMode is IOffMode),
                Animations = dataModels.ToArray(),
                //CurrentAnimation = (string)OperationManager.Instance.GetProperty("AnimationMode", "CurrentAnimation"),
                CurrentState = (AnimationState)OperationManager.Instance.GetProperty("AnimationMode", "CurrentState")
            };
            return View(model);
        }
        [HttpGet("power")]
        public IActionResult Power() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new PowerModel {
                PoweredOff = (OperationManager.Instance.CurrentOperatingMode is IOffMode)
            };
            return View(model);
        }
        [HttpGet("schedule")]
        public IActionResult Schedule() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new ScheduleModel(ConfigurationManager.Instance.CurrentSchedule);
            return View(model);
        }
        [HttpGet("schedule_old")]
        public IActionResult ScheduleOld() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            var model = new ScheduleModel(ConfigurationManager.Instance.CurrentSchedule);
            return View("Schedule_old", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
