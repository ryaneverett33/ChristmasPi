using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Util;
using ChristmasPi.Operations.Interfaces;
using ChristmasPi.Operations;
using ChristmasPi.Animation;

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimationsController : ControllerBase
    {
        [HttpPost("play")]
        public IActionResult PlayAnimation(string animation) {
            // /api/animations/play
            if (animation == null) {
                // allow resume if animation is null
                if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                    return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
                return new StatusCodeResult((OperationManager.Instance.CurrentOperatingMode as IAnimationMode).ResumeAnimation());
            }
            if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                OperationManager.Instance.SwitchModes("AnimationMode");
            int status = (OperationManager.Instance.CurrentOperatingMode as IAnimationMode).StartAnimation(animation);
            return new StatusCodeResult(status);
        }
        [HttpPost("pause")]
        public IActionResult PauseAnimation() {
            // /api/animations/pause
            if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            int status = (OperationManager.Instance.CurrentOperatingMode as IAnimationMode).PauseAnimation();
            return new StatusCodeResult(status);
        }
        [HttpPost("stop")]
        public IActionResult StopAnimation() {
            // /api/animations/stop
            if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            int status = (OperationManager.Instance.CurrentOperatingMode as IAnimationMode).StopAnimation();
            if (status == StatusCodes.Status200OK) {
                // return back to solid color mode
                OperationManager.Instance.SwitchModes("SolidColorMode");
            }
            return new StatusCodeResult(status);
        }
        [HttpGet()]
        public IActionResult GetAnimations() {
            // /api/animations/
            string[] animations = AnimationManager.Instance.GetAnimations();
            return new JsonResult(animations);
        }
    }
}