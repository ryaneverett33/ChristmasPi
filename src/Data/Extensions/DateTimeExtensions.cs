using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChristmasPi.Data.Exceptions;

namespace ChristmasPi.Data.Extensions {
    public static class DateTimeExtensions {
        public static DateTime FromTimestamp(this DateTime time, string timestamp) {
            if (Regex.IsMatch(timestamp, Constants.REGEX_AMPM_FORMAT))
                return time.FromAMPM(timestamp);
            else if (Regex.IsMatch(timestamp, Constants.REGEX_24HR_FORMAT))
                return time.From24Hr(timestamp);
            throw new InvalidTimestampException();
        }
        
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
        public static DateTime From24Hr(this DateTime time, string timestamp) {
            // 23:00 
            int[] hourMinute = timestamp.Split(':').Select(s => { return int.Parse(s); }).ToArray();
            return new DateTime(1, 1, 1, hourMinute[0], hourMinute[1], 0);
        }
    }
}
