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
        /// <param name="dayID">number from [0-6] for the day</param>
        /// <returns>Monday[0] - Sunday[6]</returns>
        public string GetDay(int dayID) {
            switch (dayID) {
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
        /// <param name="dayID">number from [0-6] for the day</param>
        /// <returns>M[0] - Su[6]</returns>
        public string GetDayShort(int dayID) {
            switch (dayID) {
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
        /// <param name="index">The zero-based hour of the day, [0-23]</param>
        /// <returns>1:00 AM for index=0, 1:00 Pm for index=13</returns>
        public string GetTimePeriod(int index) {
            DateTime time = new DateTime(1, 1, 1, index, 0, 0);
            return time.ToString("h tt");
        }
        public ColoredRule GetFirstRuleAt(int i, int j) {
            ColoredRule[] array = getRuleArrayByIndex(i);
            if (array == null)
                return null;
            foreach (ColoredRule rule in array) {
                if (rule.StartTime.Hour == j)
                    return rule;
                if (rule.StartTime.Hour < j && rule.EndTime.Hour > j)
                    return rule;
            }
            return null;
        }
        public int GetRuleCountAt(int i, int j) {
            int count = 0;
            ColoredRule[] array = getRuleArrayByIndex(i);
            if (array == null)
                return count;
            foreach (ColoredRule rule in array) {
                if (rule.StartTime.Hour == j)
                    count++;
                else if (rule.StartTime.Hour < j && rule.EndTime.Hour > j)
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
            bool contains = true;
            Color color = Color.Aquamarine;
            while (contains) {
                color = RandomColor.RandomKnownColorGenerator();
                contains = usedColors.Contains(color) || color == Color.White; 
            }
            usedColors.Add(color);
            return color;
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
    }
    public enum RuleStyle {
        SingleRule,     // A rule that spans only an hour
        RuleTop,        // The top of a multi-hour spanning rule
        RuleMiddle,     // The middle of a multi-hour spanning rule
        RuleEnd         // The end of a multi-hour spanning rule
    }
}
