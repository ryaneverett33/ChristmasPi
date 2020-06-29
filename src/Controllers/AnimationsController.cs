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
        public IActionResult PlayAnimation([FromBody]AnimationPlayArgument argument) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // /api/animations/play
            if (argument == null) {
                // allow resume if animation is null
                if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                    return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
                return new StatusCodeResult((OperationManager.Instance.CurrentOperatingMode as IAnimationMode).ResumeAnimation());
            }
            if (argument.animation == null)
                return new BadRequestObjectResult("Animation argument is empty");
            if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                OperationManager.Instance.SwitchModes("AnimationMode");
            int status = (OperationManager.Instance.CurrentOperatingMode as IAnimationMode).StartAnimation(argument.animation);
            return new StatusCodeResult(status);
        }
        [HttpPost("pause")]
        public IActionResult PauseAnimation() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            // /api/animations/pause
            if (OperationManager.Instance.CurrentOperatingModeName != "AnimationMode")
                return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            int status = (OperationManager.Instance.CurrentOperatingMode as IAnimationMode).PauseAnimation();
            return new StatusCodeResult(status);
        }
        [HttpPost("stop")]
        public IActionResult StopAnimation() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
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
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            // /api/animations/
            string[] animations = AnimationManager.Instance.GetAnimations();
            return new JsonResult(animations);
        }
    }
    public class AnimationPlayArgument {
        public string animation { get; set; }
    }
}