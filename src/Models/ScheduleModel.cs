using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ChristmasPi.Util;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Scheduler;

namespace ChristmasPi.Models {
    public class ScheduleModel : BaseModel {
        public ColoredRule[] Monday { get; set; }
        public ColoredRule[] Tuesday { get; set; }
        public ColoredRule[] Wednesday { get; set; }
        public ColoredRule[] Thursday { get; set; }
        public ColoredRule[] Friday { get; set; }
        public ColoredRule[] Saturday { get; set; }
        public ColoredRule[] Sunday { get; set; }

        public ScheduleModel(WeekSchedule schedule) {
            if (schedule == null)
                throw new ArgumentNullException("schedule");
            // convert schedule to individual ColoredRule arrays
            List<Color> usedColors = new List<Color>();
            Monday = scheduleToColoredRule(schedule.Monday, ref usedColors);
            Tuesday = scheduleToColoredRule(schedule.Tuesday, ref usedColors);
            Wednesday = scheduleToColoredRule(schedule.Wednesday, ref usedColors);
            Thursday = scheduleToColoredRule(schedule.Thursday, ref usedColors);
            Friday = scheduleToColoredRule(schedule.Friday, ref usedColors);
            Saturday = scheduleToColoredRule(schedule.Saturday, ref usedColors);
            Sunday = scheduleToColoredRule(schedule.Sunday, ref usedColors);
        }

        /// <summary>
        /// Gets the day of the week from its numeric ID
        /// </summary>
        /// <param name="day">number from [0-6] for the day</param>
        /// <returns>Monday[0] - Sunday[6]</returns>
        public string GetDay(int day) {
            switch (day) {
                case 0:
                    return "Monday";
                case 1:
                    return "Tuesday";
                case 2:
                    return "Wednesday";
                case 3:
                    return "Thursday";
                case 4:
                    return "Friday";
                case 5:
                    return "Saturday";
                default:
                    return "Sunday";
            }
        }
        
        /// <summary>
        /// Gets the shortened form of the day of the week from its numeric ID
        /// </summary>
        /// <param name="day">number from [0-6] for the day</param>
        /// <returns>M[0] - Su[6]</returns>
        public string GetDayShort(int day) {
            switch (day) {
                case 0:
                    return "M";
                case 1:
                    return "T";
                case 2:
                    return "W";
                case 3:
                    return "Th";
                case 4:
                    return "F";
                case 5:
                    return "Sa";
                default:
                    return "Su";
            }
        }
        
        /// <summary>
        /// Converts the hour of the day to a time string
        /// </summary>
        /// <param name="hour">The zero-based hour of the day, [0-23]</param>
        /// <returns>1:00 AM for index=0, 1:00 Pm for index=13</returns>
        public string GetTimePeriod(int hour) {
            DateTime time = new DateTime(1, 1, 1, hour, 0, 0);
            return time.ToString("h tt");
        }

        /// <summary>
        /// Gets the first rule that occurs for a given hour of a given day
        /// </summary>
        /// <param name="day">The integer representation of the day</param>
        /// <param name="hour">The integer representation of the hour</param>
        /// <returns>The chronologically first rule that occurs in the hour</returns>
        public ColoredRule GetFirstRuleAt(int day, int hour) {
            ColoredRule[] array = getRuleArrayByIndex(day);
            if (array == null)
                return null;
            foreach (ColoredRule rule in array) {
                if (rule.StartTime.Hour == hour)
                    return rule;
                if (rule.StartTime.Hour < hour && rule.EndTime.Hour > hour)
                    return rule;
            }
            return null;
        }

        /// <summary>
        /// Gets the number of rules that occur in a given hour of a given day
        /// </summary>
        /// <param name="day">The integer representation of the day</param>
        /// <param name="hour">The integer representation of the hour</param>
        /// <returns>0 if no rules exist, else the number of rules that start or occur in the hour</returns>
        public int GetRuleCountAt(int day, int hour) {
            int count = 0;
            ColoredRule[] array = getRuleArrayByIndex(day);
            if (array == null)
                return count;
            foreach (ColoredRule rule in array) {
                if (rule.StartTime.Hour == hour)
                    count++;
                else if (rule.StartTime.Hour < hour && rule.EndTime.Hour > hour)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Gets the styling info for a rule
        /// </summary>
        /// <param name="day">The integer representation of the day</param>
        /// <param name="hour">The integer representation of the hour</param>
        /// <returns>Info about how to style the rule</returns>
        public RuleStyle? GetStyleFor(int day, int hour) {
            ColoredRule[] array = getRuleArrayByIndex(day);
            if (array == null)
                return null;
            ColoredRule rule = array.FirstOrDefault<ColoredRule>(rule => 
               rule.StartTime.Hour == hour || 
                   (rule.StartTime.Hour < hour && rule.EndTime.Hour > hour)
            );
            if (rule == null)
                return null;
            if (rule.StartTime.Hour == hour) {
                if (rule.EndTime.Hour == hour)
                    return RuleStyle.SingleRule;
                return RuleStyle.RuleTop;
            }
            else if (rule.EndTime.Hour == hour + 1) {
                return RuleStyle.RuleEnd;
            }
            else
                return RuleStyle.RuleMiddle;
        }

        /// <summary>
        /// Gets the CSS border styling for the style of a rule
        /// </summary>
        /// <param name="style">Styling info for the rule</param>
        /// <returns>The CSS rule for the style of border to use, null if no rule exists</returns>
        /// <example>If the hour is the start of the rule, then border-top-right-left will be returned. 
        /// If the hour is the complete rule, then border-full will be returned.</example>
        public string GetBorderStyleFor(RuleStyle? style) {
            switch (style.Value) {
                case RuleStyle.RuleEnd:
                    return "border-bottom-right-left";
                case RuleStyle.RuleMiddle:
                    return "border-right-left";
                case RuleStyle.RuleTop:
                    return "border-top-right-left";
                default:
                    return "border-full";
            }
        }

        /// <summary>
        /// Checks if a hour is at the top of the rule span
        /// </summary>
        /// <param name="day">The integer representation of the day</param>
        /// <param name="hour">The integer representation of the hour</param>
        /// <returns>True if the hour is equal to the StartTime of the rule</returns>
        public bool IsRuleTop(int day, int hour) {
            ColoredRule rule = GetFirstRuleAt(day, hour);
            return rule == null ? false : rule.StartTime.Hour == hour;
        }

        /// <summary>
        /// Checks if a hour is at the bottom of the rule span
        /// </summary>
        /// <param name="day">The integer representation of the day</param>
        /// <param name="hour">The integer representation of the hour</param>
        /// <returns>True if the hour is equal to the EndTime* of the rule</returns>
        public bool IsRuleBottom(int day, int hour) {
            ColoredRule rule = GetFirstRuleAt(day, hour);
            return rule == null ? false : rule.EndTime.Hour == hour + 1;
        }

        /// <summary>
        /// Checks if a grid location is the middle of a given rule's span
        /// </summary>
        /// <param name="day">The integer representation of the day</param>
        /// <param name="hour">The integer representation of the hour</param>
        /// <returns>True if a rule exists and the location is it's middle, false otherwise</returns>
        /// <example>If a rule spans from 1PM-4PM (starts on 1, ends at 4) then its' middle is 2.
        /// If a rule spans from 1PM-2PM, then its' middle is 1.</example>
        public bool IsRuleMiddle(int day, int hour) {
            ColoredRule rule = GetFirstRuleAt(day, hour);
            int middleHour = 0;
            if (rule.StartTime.Hour == rule.EndTime.Hour)
                middleHour = rule.StartTime.Hour;
            else if (rule.StartTime.Hour == rule.EndTime.Hour - 1)
                middleHour = rule.StartTime.Hour;
            else {
                // get the number of hours the rule spans
                int spanMagnitude = rule.EndTime.Hour - rule.StartTime.Hour;
                int half = spanMagnitude / 2;       // use integer division to round down
                middleHour = rule.StartTime.Hour + half;
            }
            return middleHour == hour;
        }

        private ColoredRule[] getRuleArrayByIndex(int i) {
            switch (i) {
                case 0:
                    return Monday;
                case 1:
                    return Tuesday;
                case 2:
                    return Wednesday;
                case 3:
                    return Thursday;
                case 4:
                    return Friday;
                case 5:
                    return Saturday;
                default:
                    return Sunday;
            }
        }
        private ColoredRule[] scheduleToColoredRule(Schedule schedule, ref List<Color> usedColors) {
            if (schedule == null || schedule.RuleCount == 0)
                return new ColoredRule[0];
            TimeSlot[] rules = schedule.GetRules();
            ColoredRule[] newRules = new ColoredRule[rules.Length];
            for (int i = 0; i < rules.Length; i++) {
                newRules[i] = new ColoredRule {
                    StartTime = rules[i].StartTime,
                    EndTime = rules[i].EndTime,
                    Color = getNewColor(ref usedColors)
                };
            }
            return newRules;
        }
        private Color getNewColor(ref List<Color> usedColors) {
            return RandomColor.RandomColorNotInTable(ref usedColors, ColorTable.UIColors);
        }
    }
    public class ColoredRule {
        public DateTime StartTime;
        public DateTime EndTime;
        public Color Color;
        
        public string GetColorCode() {
            return Util.ColorConverter.ToHex(Color);
        }
        public override string ToString() {
            return String.Format("{0} - {1}", StartTime.ToString("h:mm"), EndTime.ToString("h:mm"));
        }
        public string GetName(int day) => $"d{day}s{StartTime.Hour}e{EndTime.Hour}";
        public string GetStartTime() => StartTime.ToString("h:mm");
        public string GetEndTime() => EndTime.ToString("h:mm");
    }
    public enum RuleStyle {
        SingleRule,     // A rule that spans only an hour
        RuleTop,        // The top of a multi-hour spanning rule
        RuleMiddle,     // The middle of a multi-hour spanning rule
        RuleEnd         // The end of a multi-hour spanning rule
    }
}
