using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChristmasPi.Scheduler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChristmasPi.Data;

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
                        dayarray.Add($"{time.StartTime} - {time.EndTime}");
                    }
                }
                jarr.Add(dayarray);
            }
            return Content(jarr.ToString(), "application/json");
        }
    }
}