using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ChristmasPi.Scheduler.Models;
using ChristmasPi.Data;
using ChristmasPi.Util;
using ChristmasPi.Data.Extensions;

namespace ChristmasPi.Scheduler {
    class Program {
        static void Main(string[] args) {
            Scheduler scheduler = new Scheduler(args);
            scheduler.Run();
        }
    }
    public class Scheduler {
        public bool Running;
        public bool Scheduling;

        private FileSystemWatcher watcher;
        private WeekSchedule schedule;
        private CancellationTokenSource currentSleepToken;
        private string apiURL;
        private HttpClient httpClient;
        private TimeSlot? lastRule;

        public Scheduler(string[] args) {
            watcher = new FileSystemWatcher(Constants.SCHEDULE_FILE);
            watcher.Changed += FileChanged;
            watcher.Created += FileChanged;
            watcher.Deleted += FileChanged;
            Running = true;
            Scheduling = false;
            apiURL = $"http://localhost:{Constants.PORT}/api";
            httpClient = new HttpClient();
        }

        public async void Run() {
            while (Running) {
                // get schedule info
                if (loadSchedule()) {
                    Scheduling = true;
                    while(Scheduling) {
                        TimeSlot[] currentRules = getCurrentSchedule().GetRules();
                        if (currentRules.Length == 0) {
                            // long sleep
                            currentSleepToken = ThreadHelpers.RegisterWakeUp();
                            await ThreadHelpers.SafeSleep(currentSleepToken, Constants.SCHEDULER_LONG_SLEEP);            // ignore the result but wakeup if need be
                            currentSleepToken = null;
                        }
                        else {
                            // get current rule
                            int closestRuleIndex = getClosestRule(currentRules);
                            if (closestRuleIndex == -1) {
                                // no more rules for the current day, go to sleep
                                currentSleepToken = ThreadHelpers.RegisterWakeUp();
                                await ThreadHelpers.SafeSleep(currentSleepToken, Constants.SCHEDULER_LONG_SLEEP);            // ignore the result but wakeup if need be
                                currentSleepToken = null;
                            }
                            else {
                                // check if we're in the rule or need to wait for the rule
                                DateTime current = DateTime.Now.ZeroOut();
                                if (current < currentRules[closestRuleIndex].StartTime) {
                                    // check if we just left a rule
                                    if (lastRule != null) {
                                        // turn off
                                    }
                                    // wait for rule
                                }
                                else {
                                    // check if we've already turned on 
                                    if (lastRule != null && lastRule.Value == currentRules[closestRuleIndex]) {
                                        // wait for the end of the rule
                                    }
                                    else {
                                        // turn on
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    // sleep for a while and then try again
                    currentSleepToken = ThreadHelpers.RegisterWakeUp();
                    await ThreadHelpers.SafeSleep(currentSleepToken, Constants.SCHEDULER_ERR_SLEEP);            // ignore the result but wakeup if need be
                    currentSleepToken = null;
                }
            }
        }

        // File Changed handler
        private void FileChanged(object sender, FileSystemEventArgs e) {
            // For each event (Created, Changed, Deleted), the action is the same: restart the scheduler and wakeup anything sleeping
            Scheduling = false;
            if (currentSleepToken != null)
                ThreadHelpers.WakeUpThread(currentSleepToken);
        }
        
        // Turns on the christmas tree
        private async Task<bool> TurnOn() {
            try {
                HttpResponseMessage response = await httpClient.PostAsync($"{apiURL}/on", null);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (HttpRequestException e) {
                Console.WriteLine($"Failed to execute turnon command, exception: {e.Message}");
            }
            return false;
        }
        
        // Turns off the christmas tree
        private async Task<bool> TurnOff() {
            try {
                HttpResponseMessage response = await httpClient.PostAsync($"{apiURL}/off", null);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (HttpRequestException e) {
                Console.WriteLine($"Failed to execute turnon command, exception: {e.Message}");
            }
            return false;
        }

        // loads the schedule if the schedule file exists
        private bool loadSchedule() {
            if (File.Exists(Constants.SCHEDULE_FILE)) {
                try {
                    string json = File.ReadAllText(Constants.SCHEDULE_FILE);
                    schedule = JsonConvert.DeserializeObject<WeekSchedule>(json);
                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }
            else
                return false;
        }

        // gets the current day schedule
        private Schedule getCurrentSchedule() {
            switch (DateTime.Now.DayOfWeek) {
                case DayOfWeek.Monday:
                    return schedule.Monday;
                case DayOfWeek.Tuesday:
                    return schedule.Tuesday;
                case DayOfWeek.Wednesday:
                    return schedule.Wednesday;
                case DayOfWeek.Thursday:
                    return schedule.Thursday;
                case DayOfWeek.Friday:
                    return schedule.Friday;
                case DayOfWeek.Saturday:
                    return schedule.Saturday;
                case DayOfWeek.Sunday:
                    return schedule.Sunday;
            }
            return null;
        }

        // gets the closest rule to execute, if no rules are available returns -1
        private int getClosestRule(TimeSlot[] rules) {
            for (int i = 0; i < rules.Length; i++) {
                TimeSlot rule = rules[i];
                DateTime current = DateTime.Now.ZeroOut();
                if (current < rule.StartTime)
                    return i;
            }
            return -1;
        }
    }
}
