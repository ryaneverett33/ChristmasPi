using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data.Extensions;

namespace ChristmasPi.Scheduler.Models {
    public class Schedule {
        private List<TimeSlot> times;

        public int RuleCount => times.Count;

        public Schedule(ScheduleRule[] rules) {
            times = new List<TimeSlot>(rules.Length);
            if (!rules.IsSorted())
                Array.Sort(rules);
            for (int i = 0; i < rules.Length; i++) {
                times.Add(new TimeSlot(rules[i].OnTime, rules[i].OffTime));
            }
        }

        public TimeSlot[] GetRules() {
            Console.WriteLine($"Has {times.Count} rules");
            return times.ToArray();
        }
    }
    public struct TimeSlot {
        public DateTime StartTime;
        public DateTime EndTime;
        public TimeSpan Duration => EndTime - StartTime;

        public TimeSlot(DateTime start, DateTime end) {
            DateTime startRel = new DateTime(1, 1, 1, start.Hour, start.Minute, 0);
            DateTime endRel = new DateTime(1, 1, 1, end.Hour, end.Minute, 0);
            if (endRel <= startRel)
                throw new ArgumentException("End must occur after start");
            StartTime = startRel;
            EndTime = endRel;
        }
    }
}
