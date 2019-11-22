using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChristmasPi.Scheduler.Models {
    [JsonConverter(typeof(ScheduleConverter))]
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
        public Schedule() {
            times = new List<TimeSlot>();
        }

        /// <summary>
        /// Gets a flattened array of schedule rules
        /// </summary>
        /// <returns>Array of schedule rules<</returns>
        public TimeSlot[] GetRules() {
            // Console.WriteLine($"Has {times.Count} rules");
            return times.ToArray();
        }

        /// <summary>
        /// Adds a rule to the schedule if possible
        /// </summary>
        /// <param name="start">Start time of the rule</param>
        /// <param name="end">End time of the rule</param>
        /// <returns>True if successfully added, false if rule overlaps a previous rule</returns>
        public bool AddRule(DateTime start, DateTime end) {
            return AddRule(new TimeSlot(start, end));
        }

        /// <summary>
        /// Adds a rule to the schedule if possible
        /// </summary>
        /// <param name="ruleToAdd">The rule to add to the schedule</param>
        /// <returns>True if successfully added, false if rule overlaps a previous rule</returns>
        public bool AddRule(TimeSlot newRule) {
            // check if overlaps another rule
            foreach (TimeSlot rule in times) {
                if (rule.doRulesOverlap(newRule))
                    return false;
            }
            times.Add(newRule);
            times.Sort();
            return true;
        }

        /// <summary>
        /// Removes a rule from the schedule if it exists
        /// </summary>
        /// <param name="start">Start time of the rule</param>
        /// <param name="end">End time of the rule</param>
        /// <returns>True if successfully removed, false if rule doesn't exist</returns>
        public bool RemoveRule(DateTime start, DateTime end) {
            TimeSlot ruleToRemove = new TimeSlot(start, end);
            if (times.Remove(ruleToRemove)) {
                times.Sort();
                return true;
            }
            return false;
        }
    }
    [JsonConverter(typeof(TimeSlotConverter))]
    public struct TimeSlot : IComparable {
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

        public TimeSlot(string start, string end) : this(new DateTime().FromTimestamp(start),
                                                        new DateTime().FromTimestamp(end)) { }

        /// <summary>
        /// Checks whether or not do schedule rules are overlapping
        /// </summary>
        /// <param name="other">The schedule rule to compare again</param>
        /// <returns>True if they overlap, false if they don't</returns>
        public bool doRulesOverlap(TimeSlot other) {
            if (StartTime < other.StartTime && EndTime < other.EndTime)
                return true;
            else if (StartTime > other.StartTime && EndTime > other.EndTime)
                return true;
            else if (StartTime == other.StartTime && EndTime == other.EndTime)
                return true;
            return false;
        }

        /// <seealso cref="ScheduleRule.StrongCompareTo(object)"/>
        public int CompareTo(object obj) {
            if (!(obj is TimeSlot))
                return 1;
            else {
                TimeSlot other = (TimeSlot)obj;
                if (StartTime < other.StartTime)
                    return -1;
                if (StartTime > other.StartTime)
                    return 1;
                if (EndTime < other.EndTime)
                    return -1;
                if (EndTime > other.EndTime)
                    return 1;
                return 0;
            }
        }

        public override bool Equals(object obj) {
            return CompareTo(obj) == 0;
        }

        public static override bool operator ==(TimeSlot a, object b) {
            if (!(b is TimeSlot))
                return false;
            TimeSlot other = (TimeSlot)b;
            if (a.StartTime == other.StartTime && a.EndTime == other.EndTime)
                return true;
            return false;
        }

        public static bool operator !=(TimeSlot a, object b) {
            return !(a == b);
        }
    }

    public class ScheduleConverter : JsonConverter<Schedule> {
        /// <see cref="https://stackoverflow.com/a/22539730"/>
        public override Schedule ReadJson(JsonReader reader, Type objectType, Schedule existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject jo = JObject.Load(reader);
            Schedule schedule = new Schedule();

            return schedule;
        }
        public override void WriteJson(JsonWriter writer, Schedule value, JsonSerializer serializer) {
            writer.WriteStartArray();
            TimeSlotConverter converter = new TimeSlotConverter();
            foreach (TimeSlot time in value.GetRules()) {
                converter.WriteJson(writer, time, serializer);
            }
            writer.WriteEndArray();
        }
        public Schedule JTokenToObject(JToken token) {
            JArray rulesArray = (JArray)token;
            Schedule schedule = new Schedule();
            TimeSlotConverter slotConverter = new TimeSlotConverter();
            foreach (JToken ruleToken in rulesArray) {
                TimeSlot slot = slotConverter.JTokenToObject(ruleToken);
                schedule.AddRule(slot);
            }
            return schedule;
        }
    }

    public class TimeSlotConverter : JsonConverter<TimeSlot> {
        /// <see cref="https://stackoverflow.com/a/22539730"/>
        public override TimeSlot ReadJson(JsonReader reader, Type objectType, TimeSlot existingValue, bool hasExistingValue, JsonSerializer serializer) {
            JObject jo = JObject.Load(reader);
            TimeSlot slot = new TimeSlot((string)jo["start"], (string)jo["end"]);
            return slot;
        }
        public override void WriteJson(JsonWriter writer, TimeSlot value, JsonSerializer serializer) {
            writer.WriteStartObject();
            writer.WritePropertyName("start");
            writer.WriteValue(value.StartTime.ToString("HH:mm"));
            writer.WritePropertyName("end");
            writer.WriteValue(value.EndTime.ToString("HH:mm"));
            writer.WriteEndObject();
        }
        public TimeSlot JTokenToObject(JToken token) {
            string start = (string)token["start"];
            string end = (string)token["end"];
            return new TimeSlot(start, end);
        }
    }
}
