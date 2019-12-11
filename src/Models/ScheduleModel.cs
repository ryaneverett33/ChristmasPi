using System;
using System.Collections.Generic;
using System.Drawing;
using ChristmasPi.Util;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Scheduler;

namespace ChristmasPi.Models {
    public class ScheduleModel {
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

        public string GetDay(int i) {
            switch (i) {
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
        public string GetTimePeriod(int i) {
            DateTime time1 = new DateTime(1, 1, 1, i, 0, 0);
            // DateTime time2 = new DateTime(1, 1, 1, i, 59, 0);
            string part1 = time1.ToString("t");
            // string part2 = time2.ToString("t");
            return String.Format("{0}", part1);
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
}
