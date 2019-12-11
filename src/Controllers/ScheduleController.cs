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

namespace ChristmasPi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase {
        [HttpGet]
        public IActionResult Get() {
            TimeSlot[][] schedule = ConfigurationManager.Instance.CurrentSchedule.GetSimpleSchedule();
            JArray jarr = new JArray();
            for (int i = 0; i < schedule.Length; i++) {
                JArray dayarray = new JArray();
                if (schedule[i] != null) {
                    for (int j = 0; j < schedule[i].Length; j++) {
                        TimeSlot time = schedule[i][j];
                        dayarray.Add($"{time.StartTime.ToString("HH:MM")} - {time.EndTime.ToString("HH:MM")}");
                    }
                }
                jarr.Add(dayarray);
            }
            return Content(jarr.ToString(), "application/json");
        }
        [HttpPost("add")]
        public IActionResult AddRule([FromBody]ScheduleRuleArgument argument) {
            try {
                DateTime startTime = new DateTime().FromTimestamp(argument.start);
                DateTime endTime = new DateTime().FromTimestamp(argument.end);
                if (endTime < startTime || endTime == startTime)
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                if (ConfigurationManager.Instance.CurrentSchedule.AddRule(startTime, endTime, argument.repeat))
                    return new OkResult();
                else
                    return new StatusCodeResult(StatusCodes.Status405MethodNotAllowed);
            }
            catch (InvalidTimestampException) {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
        }
    }
    public class ScheduleRuleArgument {
        public string start { get; set; }
        public string end { get; set; }
        public int repeat { get; set; }
    }
}