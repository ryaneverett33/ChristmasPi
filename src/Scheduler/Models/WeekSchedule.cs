using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Scheduler.Models {
    public class WeekSchedule {

        private Schedule Monday;
        private Schedule Tuesday;
        private Schedule Wednesday;
        private Schedule Thursday;
        private Schedule Friday;
        private Schedule Saturday;
        private Schedule Sunday;
        private bool createdSchedules;
        private ScheduleRule[] _rules;

        public ScheduleRule[] Rules { get { return _rules; } set {
                _rules = value;
                CreateSchedules();
            }
        }
        public WeekSchedule() {
            createdSchedules = false;
        }
        
        /// <summary>
        /// Gets a simplified version of the schedule
        /// </summary>
        /// <returns>An array where each day (Monday=0, Sunday=6) is an array of timeslots</returns>
        public TimeSlot[][] GetSimpleSchedule() {
            /// TODO
            TimeSlot[][] times = new TimeSlot[7][];
            if (Monday != null && Monday.RuleCount != 0)
                times[0] = Monday.GetRules();
            if (Tuesday != null && Tuesday.RuleCount != 0)
                times[1] = Monday.GetRules();
            if (Wednesday != null && Wednesday.RuleCount != 0)
                times[2] = Monday.GetRules();
            if (Thursday != null && Thursday.RuleCount != 0)
                times[3] = Monday.GetRules();
            if (Friday != null && Friday.RuleCount != 0)
                times[4] = Monday.GetRules();
            if (Saturday != null && Saturday.RuleCount != 0)
                times[5] = Monday.GetRules();
            if (Sunday != null && Sunday.RuleCount != 0)
                times[6] = Monday.GetRules();
            return times;
        }

        private void CreateSchedules() {
            // iterate through rules and condense into schedules
            List<ScheduleRule>[] days = new List<ScheduleRule>[7];
            if (_rules == null || _rules.Length == 0) return;
            for (int i = 0; i < days.Length; i++) { days[i] = new List<ScheduleRule>(); }
            foreach (ScheduleRule rule in _rules) {
                // assign each rule to its list by repeat values
                if (rule.repeats.HasFlag(RepeatUsage.RepeatMonday))
                    days[0].Add(rule);
                else if (rule.repeats.HasFlag(RepeatUsage.RepeatTuesday))
                    days[1].Add(rule);
                else if (rule.repeats.HasFlag(RepeatUsage.RepeatWednesday))
                    days[2].Add(rule);
                else if (rule.repeats.HasFlag(RepeatUsage.RepeatThursday))
                    days[3].Add(rule);
                else if (rule.repeats.HasFlag(RepeatUsage.RepeatFriday))
                    days[4].Add(rule);
                else if (rule.repeats.HasFlag(RepeatUsage.RepeatSaturday))
                    days[5].Add(rule);
                else if (rule.repeats.HasFlag(RepeatUsage.RepeatSunday))
                    days[6].Add(rule);
                else {
                    Console.WriteLine("INVALID Schedule Rule, no day set!");
                }
            }
            // sort each list
            for (int i = 0; i < days.Length; i++) { 
                days[i].Sort(delegate(ScheduleRule a, ScheduleRule b) {
                    int comparison = a.StrongCompareTo(b);
                    switch (comparison) {
                        case -1:
                        case 0:
                        case 1:
                            return comparison;
                        // handle special cases
                        case 10:
                            throw new InvalidOperationException("Can't compare two objects of different type");
                        default:
                            return comparison;
                    }
                });
            }
            if (days[0].Count != 0)
                Monday = new Schedule(days[0].ToArray());
            if (days[1].Count != 0)
                Tuesday = new Schedule(days[1].ToArray());
            if (days[2].Count != 0)
                Wednesday = new Schedule(days[2].ToArray());
            if (days[3].Count != 0)
                Thursday = new Schedule(days[3].ToArray());
            if (days[4].Count != 0)
                Friday = new Schedule(days[4].ToArray());
            if (days[5].Count != 0)
                Saturday = new Schedule(days[5].ToArray());
            if (days[6].Count != 0)
                Sunday = new Schedule(days[6].ToArray());
        }

        /// <summary>
        /// Creates the default schedule (empty)
        /// </summary>
        /// <returns>A new WeekSchedule object with default values</returns>
        public static WeekSchedule DefaultSchedule() {
            WeekSchedule @default = new WeekSchedule();
            @default.Rules = null;
            return @default;
        }
    }
}
