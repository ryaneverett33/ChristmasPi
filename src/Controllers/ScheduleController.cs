using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Data.Models.Scheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChristmasPi.Data;
using ChristmasPi.Data.Extensions;
using ChristmasPi.Data.Exceptions;
using Serilog;

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase {
        [HttpGet]
        public IActionResult Get() {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "get") is IActionResult redirect)
                return redirect;
            TimeSlot[][] schedule = ConfigurationManager.Instance.CurrentSchedule.GetSimpleSchedule();
            JArray jarr = new JArray();
            for (int i = 0; i < schedule.Length; i++) {
                JArray dayarray = new JArray();
                if (schedule[i] != null) {
                    for (int j = 0; j < schedule[i].Length; j++) {
                        TimeSlot time = schedule[i][j];
                        dayarray.Add($"{time.StartTime.ToString("HH:mm")} - {time.EndTime.ToString("HH:mm")}");
                    }
                }
                jarr.Add(dayarray);
            }
            Log.ForContext<ScheduleController>().Debug("Get() returned {schedule}", jarr);
            return Content(jarr.ToString(), "application/json");
        }
        [HttpPost("add")]
        public IActionResult AddRule([FromBody]ScheduleAddRuleArgument argument) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            try {
                if (argument == null) {
                    Log.ForContext<ScheduleController>().Debug("AddRule(), no arguments");
                    return new BadRequestObjectResult("No arguments supplied");
                }   
                if (argument.start == null || argument.end == null) {
                    Log.ForContext<ScheduleController>().Debug("AddRule(), no time arguments");
                    return new BadRequestObjectResult("Invalid time arguments");
                }
                if (argument.repeat == 0) {
                    Log.ForContext<ScheduleController>().Debug("AddRule(), repeat argument is 0");
                    return new BadRequestObjectResult("Invalid repeat argument");
                }
                DateTime startTime = new DateTime().FromTimestamp(argument.start);
                DateTime endTime = new DateTime().FromTimestamp(argument.end);
                if (endTime < startTime || endTime == startTime) {
                    Log.ForContext<ScheduleController>().Debug("AddRule(), startTime {} or endTime {} constitutes an illegal rule", startTime, endTime);
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                if (ConfigurationManager.Instance.CurrentSchedule.AddRule(startTime, endTime, argument.repeat)) {
                    Log.ForContext<ScheduleController>().Debug("AddRule(), successfully added rule from {start} to {end}", startTime, endTime);
                    return new OkResult();
                }
                else {
                    Log.ForContext<ScheduleController>().Debug("AddRule(), failed to add rule from {start} to {end}", startTime, endTime);
                    return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
                }
            }
            catch (InvalidTimestampException e) {
                Log.ForContext<ScheduleController>().Error(e, "AddRule() encountered an exception");
                return new BadRequestObjectResult("Invalid argument: time can't be parsed");
            }
        }
        [HttpPost("remove")]
        public IActionResult RemoveRule([FromBody]ScheduleRemoveRuleArgument argument) {
            if (RedirectHandler.ShouldRedirect(this.RouteData, "post") is IActionResult redirect)
                return redirect;
            try {
                if (argument == null) {
                    Log.ForContext<ScheduleController>().Debug("RemoveRule(), no arguments");
                    return new BadRequestObjectResult("No arguments supplied");
                }
                if (argument.start == null || argument.end == null) {
                    Log.ForContext<ScheduleController>().Debug("RemoveRule(), no time arguments");
                    return new BadRequestObjectResult("Invalid time arguments");
                }
                if (argument.day == null) {
                    Log.ForContext<ScheduleController>().Debug("RemoveRule(), no day argument");
                    return new BadRequestObjectResult("Invalid day argument");
                }
                DateTime startTime = new DateTime().FromTimestamp(argument.start);
                DateTime endTime = new DateTime().FromTimestamp(argument.end);
                if (!ConfigurationManager.Instance.CurrentSchedule.RuleExists(startTime, endTime, argument.GetRepeatFromDay())) {
                    Log.ForContext<ScheduleController>().Debug("RemoveRule(), rule at {start} to {end} on {day} doesn't exist", startTime, endTime, argument.day);
                    return new BadRequestObjectResult("Rule doesn't exist");
                }
                else {
                    if (ConfigurationManager.Instance.CurrentSchedule.RemoveRule(startTime, endTime, argument.GetRepeatFromDay(), ignoreErrors: true)) {
                        new Task(() => ConfigurationManager.Instance.SaveSchedule()).Start();
                        Log.ForContext<ScheduleController>().Debug("RemoveRule(), successfully removed rule at {start} to {end} on {day}", startTime, endTime, argument.day);
                        return new OkResult();
                    }
                    else {
                        Log.ForContext<ScheduleController>().Debug("RemoveRule(), failed to remove rule at {start} to {end} on {day}", startTime, endTime, argument.day);
                        return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
                    }
                }
            }
            catch (InvalidTimestampException e) {
                Log.ForContext<ScheduleController>().Error(e, "RemoveRule() encountered an exception");
                return new BadRequestObjectResult("Invalid argument: time can't be parsed");
            }
        }
    }
    public class ScheduleAddRuleArgument {
        public string start { get; set; }
        public string end { get; set; }
        public int repeat { get; set; }
    }
    public class ScheduleRemoveRuleArgument {
        public string start { get; set; }
        public string end { get; set; }
        public string day { get; set; }

        /// <summary>
        /// Converts the day argument to an integer representation of RepeatUsage
        /// </summary>
        /// <seealso cref="RepeatUsage"/>
        /// <returns>An integer representing RepeatUsage</returns>
        public int GetRepeatFromDay() {
            if (day.Equals("sunday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatSunday;
            else if (day.Equals("monday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatMonday;
            else if (day.Equals("tuesday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatTuesday;
            else if (day.Equals("wednesday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatWednesday;
            else if (day.Equals("thursday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatThursday;
            else if (day.Equals("friday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatFriday;
            else if (day.Equals("saturday", StringComparison.CurrentCultureIgnoreCase))
                return (int)RepeatUsage.RepeatSaturday;
            return 0;
        }
    }
}