﻿using System;
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
                CurrentColor = (Color)OperationManager.Instance.GetProperty("SolidColorMode", "CurrentColor"),
                DefaultColor = ConfigurationManager.Instance.CurrentTreeConfig.tree.color.DefaultColor
            };
            return View(model);
        }

        [HttpGet("animation")]
        public IActionResult Animation() {
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
            var model = new PowerModel {
                PoweredOff = (OperationManager.Instance.CurrentOperatingMode is IOffMode)
            };
            return View(model);
        }
        [HttpGet("schedule")]
        public IActionResult Schedule() {
            var model = new ScheduleModel(ConfigurationManager.Instance.CurrentSchedule);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
