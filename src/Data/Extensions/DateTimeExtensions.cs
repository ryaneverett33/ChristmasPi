using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Extensions {
    public static class DateTimeExtensions {

        /// <summary>
        /// Converts a timestamp to its DateTime representation
        /// </summary>
        /// <param name="timestamp">The timestamp to convert (in 24hr format or meridian format)</param>
        /// <returns>A new DateTime representing the timestamp</returns>
        public static DateTime FromTimestamp(this DateTime time, string timestamp) {
            if (Regex.IsMatch(timestamp, Constants.REGEX_AMPM_FORMAT))
                return time.FromAMPM(timestamp);
            else if (Regex.IsMatch(timestamp, Constants.REGEX_24HR_FORMAT))
                return time.From24Hr(timestamp);
            throw new InvalidTimestampException();
        }

        /// <summary>
        /// Converts a meridian format timestamp to its DateTime representation
        /// </summary>
        /// <param name="timestamp">Timestamp in meridian format</param>
        /// <returns>A new DateTime representing the timestamp</returns>
        public static DateTime FromAMPM(this DateTime time, string timestamp) {
            // !2:00 AM -> 00:00
            // 1:30 PM -> 13:30
            string[] split = timestamp.Split(' ');
            if (split.Length != 2)
                throw new InvalidTimestampException();
            int[] hourMinute = split[0].Split(':').Select(s => { return int.Parse(s); }).ToArray();
            int hour, minute = hourMinute[1];
            if (split[1].Trim().Equals("AM")) {
                // handle case hour=12
                if (hourMinute[0] == 12)
                    hour = 0;
                else
                    hour = hourMinute[0];
            }
            else if (split[1].Trim().Equals("PM")) {
                // handle case hour=12
                if (hourMinute[0] == 12)
                    hour = 12;
                else
                    hour = hourMinute[0] + 12;
            }
            else {
                throw new InvalidTimestampException();
            }
            return new DateTime(1, 1, 1, hour, minute, 0);
        }

        /// <summary>
        /// Converts a 24hr timestamp to its DateTime representation
        /// </summary>
        /// <param name="timestamp">Timestamp in 24hr format</param>
        /// <returns>A new DateTime representing the timestamp</returns>
        public static DateTime From24Hr(this DateTime time, string timestamp) {
            // 23:00 
            int[] hourMinute = timestamp.Split(':').Select(s => { return int.Parse(s); }).ToArray();
            return new DateTime(1, 1, 1, hourMinute[0], hourMinute[1], 0);
        }
        
        /// <summary>
        /// Zeros out the day/month/year of a datetime for comparison against DateTime objects without a set date
        /// </summary>
        /// <returns>A new datetime object with zeroed out day/month/year values</returns>
        public static DateTime ZeroOut(this DateTime time) {
            int ms = time.Millisecond;
            int s = time.Second;
            int m = time.Minute;
            int h = time.Hour;
            return new DateTime(1, 1, 1, h, m, s, ms);
        }
    }
}
