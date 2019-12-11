using System;
using System.Text.RegularExpressions;
using ChristmasPi.Data.Extensions;
using Newtonsoft.Json;

namespace ChristmasPi.Data.Models.Scheduler {
    public class ScheduleRule : IComparable {
        [JsonIgnore]
        public DateTime OnTime { get; set; }
        [JsonIgnore]
        public DateTime OffTime { get; set; }

        public string start { 
            get { 
                return OnTime.ToString(); 
            }
            set { 
                OnTime = OnTime.FromTimestamp(value); 
            } 
        }
        public string stop {
            get {
                return OffTime.ToString();
            }
            set {
                OffTime = OffTime.FromTimestamp(value);
            }
        }

        [JsonConverter(typeof(RepeatUsageConverter))]
        public RepeatUsage repeats { get; set; }

        public int CompareTo(object obj) {
            return StrongCompareTo(obj);
        }

        /// <summary>
        /// Strongly compares the time of this object and another object
        /// </summary>
        /// <param name="obj">The other object being compared against</param>
        /// <returns>10 - non-comparable</returns>
        /// <returns>-10 - overlap</returns>
        /// <returns>20 - Repeat Usage for other is less</returns>
        /// /// <returns>-20 - Repeat Usage is less than other</returns>
        /// <returns>-1 - this comes before b</returns>
        /// <returns>0 - objects are the same</returns>
        /// <returns>1 - this comes after a</returns>
        public int StrongCompareTo(object obj) {
            if (!(obj is ScheduleRule))
                return 10;
            else {
                ScheduleRule other = (ScheduleRule)obj;
                // check if same
                if (OnTime == other.OnTime && OffTime == other.OffTime)
                    return 0;
                // check for overlap
                if (OffTime > other.OnTime)
                    return -10;
                if (OffTime < other.OnTime)
                    return -1;
                if (OnTime > other.OffTime)
                    return 1;
                if (repeats < other.repeats)
                    return -20;
                if (repeats > other.repeats)
                    return 20;
                return 0;
            }
        }
    }
    [Flags]
    public enum RepeatUsage {
        RepeatMonday = 1,
        RepeatTuesday = 2,
        RepeatWednesday = 4,
        RepeatThursday = 8,
        RepeatFriday = 16,
        RepeatSaturday = 32,
        RepeatSunday = 64
    }

    /// <summary>
    /// A JsonConverter to parse between HardwareType and JSON
    /// </summary>
    /// <remarks>Only used by JSON.Net, no need to call directly</remarks>
    /// <see cref="https://stackoverflow.com/a/52526628">
    public class RepeatUsageConverter : JsonConverter<RepeatUsage> {
        public override RepeatUsage ReadJson(JsonReader reader, Type objectType, RepeatUsage existingValue, bool hasExistingValue, JsonSerializer serializer) {
            var token = reader.Value as string ?? reader.Value.ToString();
            var stripped = Regex.Replace(token, @"<[^>]+>", string.Empty);
            if (Enum.TryParse<RepeatUsage>(stripped, out var result)) {
                return result;
            }
            return default;
        }

        public override void WriteJson(JsonWriter writer, RepeatUsage value, JsonSerializer serializer) {
            writer.WriteValue((int)value);
        }
    }
}
