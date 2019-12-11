using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ChristmasPi.Data;

namespace ChristmasPi.Data.Models.Scheduler {
    [JsonConverter(typeof(WeekScheduleConverter))]
    public class WeekSchedule {

        public Schedule Monday;
        public Schedule Tuesday;
        public Schedule Wednesday;
        public Schedule Thursday;
        public Schedule Friday;
        public Schedule Saturday;
        public Schedule Sunday;
        private ScheduleRule[] _rules;

        public ScheduleRule[] Rules { get { return _rules; } set {
                _rules = value;
                CreateSchedules();
            }
        }
        public WeekSchedule() {
            Monday = new Schedule();
            Tuesday = new Schedule();
            Wednesday = new Schedule();
            Thursday = new Schedule();
            Friday = new Schedule();
            Saturday = new Schedule();
            Sunday = new Schedule();
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
                times[1] = Tuesday.GetRules();
            if (Wednesday != null && Wednesday.RuleCount != 0)
                times[2] = Wednesday.GetRules();
            if (Thursday != null && Thursday.RuleCount != 0)
                times[3] = Thursday.GetRules();
            if (Friday != null && Friday.RuleCount != 0)
                times[4] = Friday.GetRules();
            if (Saturday != null && Saturday.RuleCount != 0)
                times[5] = Saturday.GetRules();
            if (Sunday != null && Sunday.RuleCount != 0)
                times[6] = Sunday.GetRules();
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
        /// Adds a new rule to the schedule
        /// </summary>
        /// <param name="start">Start time of the rule</param>
        /// <param name="end">End time of the rule</param>
        /// <param name="repeats">Days on which to run rule (must be at least one day</param>
        /// <returns>True if added to the schedule, false if unable to add</returns>
        /// <remarks>Makes a call to ConfigurationManager.SaveSchedule upon successfully adding rule</remarks>
        public bool AddRule(DateTime start, DateTime end, int repeats) {
            if (start == end || start > end)
                return false;
            RepeatUsage repeatUsage = (RepeatUsage)repeats;
            bool added = false, error = false;
            if (repeatUsage.HasFlag(RepeatUsage.RepeatMonday)) {
                if (Monday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            else if (repeatUsage.HasFlag(RepeatUsage.RepeatTuesday) && !error) {
                if (Tuesday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            else if (repeatUsage.HasFlag(RepeatUsage.RepeatWednesday) && !error) {
                if (Wednesday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            else if (repeatUsage.HasFlag(RepeatUsage.RepeatThursday) && !error) {
                if (Thursday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            else if (repeatUsage.HasFlag(RepeatUsage.RepeatFriday) && !error) {
                if (Friday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            else if (repeatUsage.HasFlag(RepeatUsage.RepeatSaturday) && !error) {
                if (Saturday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            else if (repeatUsage.HasFlag(RepeatUsage.RepeatSunday) && !error) {
                if (Sunday.AddRule(start, end))
                    added = true;
                else
                    error = true;
            }
            if (error) {
                // remove rules
                if (!RemoveRule(start, end, repeats, true)) {
                    Console.WriteLine("LOGTHIS Failed to remove overlapping rule from schedule");
                    Console.WriteLine("Schedule may be corrupted");
                    return false;
                }
                return false;
            }
            if (added)
                Task.Run(() => {
                    ConfigurationManager.Instance.SaveSchedule();
                });
            return added;
        }

        /// <summary>
        /// Removes a rule from the schedule
        /// </summary>
        /// <param name="start">Start time of the rule</param>
        /// <param name="end">End time of the rule</param>
        /// <param name="repeats">Days on which to run rule (must be at least one day</param>
        /// <returns>True if removed from the schedule, false if unable to remove</returns>
        /// <remarks>Makes a call to ConfigurationManager.SaveSchedule upon successfully removing rule</remarks>
        public bool RemoveRule(DateTime start, DateTime end, int repeats, bool ignoreErrors = false) {
            if (start == end || start > end)
                return false;
            RepeatUsage repeatUsage = (RepeatUsage)repeats;
            bool removed = false, error = false;
            if (repeatUsage.HasFlag(RepeatUsage.RepeatMonday)) {
                if (Monday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (repeatUsage.HasFlag(RepeatUsage.RepeatTuesday) && (!error || ignoreErrors)) {
                if (Tuesday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (repeatUsage.HasFlag(RepeatUsage.RepeatWednesday) && (!error || ignoreErrors)) {
                if (Wednesday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (repeatUsage.HasFlag(RepeatUsage.RepeatThursday) && (!error || ignoreErrors)) {
                if (Thursday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (repeatUsage.HasFlag(RepeatUsage.RepeatFriday) && (!error || ignoreErrors)) {
                if (Friday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (repeatUsage.HasFlag(RepeatUsage.RepeatSaturday) && (!error || ignoreErrors)) {
                if (Saturday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (repeatUsage.HasFlag(RepeatUsage.RepeatSunday) && (!error || ignoreErrors)) {
                if (Sunday.RemoveRule(start, end))
                    removed = true;
                else
                    error = true;
            }
            if (error && !ignoreErrors) {
                Console.WriteLine("LOGTHIS An error occurred removing a rule from the schedule");
                Console.WriteLine("Schedule may be corrupted");
                return false;
            }
            return removed;
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
    public class WeekScheduleConverter : JsonConverter<WeekSchedule> {
        /// <see cref="https://stackoverflow.com/a/22539730"/>
        public override WeekSchedule ReadJson(JsonReader reader, Type objectType, WeekSchedule existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject jo = JObject.Load(reader);
            WeekSchedule schedule = new WeekSchedule();
            JArray rulesArr = (JArray)jo["schedule"];
            ScheduleConverter schedConverter = new ScheduleConverter();
            for (int i = 0; i < rulesArr.Count; i++) {
                Schedule daySchedule = schedConverter.JTokenToObject(rulesArr[i]);
                switch (i) {
                    case 0:
                        schedule.Monday = daySchedule;
                        break;
                    case 1:
                        schedule.Tuesday = daySchedule;
                        break;
                    case 2:
                        schedule.Wednesday = daySchedule;
                        break;
                    case 3:
                        schedule.Thursday = daySchedule;
                        break;
                    case 4:
                        schedule.Friday = daySchedule;
                        break;
                    case 5:
                        schedule.Saturday = daySchedule;
                        break;
                    case 6:
                        schedule.Sunday = daySchedule;
                        break;
                }
            }
            return schedule;
        }
        public override void WriteJson(JsonWriter writer, WeekSchedule value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("schedule");
            writer.WriteStartArray();
            ScheduleConverter converter = new ScheduleConverter();
            converter.WriteJson(writer, value.Monday, serializer);
            converter.WriteJson(writer, value.Tuesday, serializer);
            converter.WriteJson(writer, value.Wednesday, serializer);
            converter.WriteJson(writer, value.Thursday, serializer);
            converter.WriteJson(writer, value.Friday, serializer);
            converter.WriteJson(writer, value.Saturday, serializer);
            converter.WriteJson(writer, value.Sunday, serializer);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
