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
        public string ScheduleFileLoc { get; private set; }

        private FileSystemWatcher watcher;
        private WeekSchedule schedule;
        private CancellationTokenSource currentSleepToken;
        private string apiURL;
        private HttpClient httpClient;
        private TimeSlot? lastRule;

        public Scheduler(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Equals("--config", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-config", StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 < args.Length) {
                        ScheduleFileLoc = args[i + 1];
                        break;
                    }
                    Console.WriteLine("Invalid config argument");
                    printHelp();
                }
                if (args[i].Equals("--h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("help", StringComparison.CurrentCultureIgnoreCase))
                    printHelp();
            }
            if (ScheduleFileLoc == null)
                ScheduleFileLoc = Constants.SCHEDULE_FILE;
            watcher = new FileSystemWatcher(ScheduleFileLoc);
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
                                        lastRule = null;
                                        for (int i = 0; i < Constants.SCHEDULER_MAX_ATTEMPTS; i++) {
                                            bool success = await TurnOff();
                                            if (success)
                                                break;
                                            else {
                                                Console.WriteLine($"Failed to turnon, {i+1}/{Constants.SCHEDULER_MAX_ATTEMPTS}");
                                            }
                                        }
                                    }
                                    else {
                                        // wait for rule to start
                                        TimeSpan sleepTime = currentRules[closestRuleIndex].StartTime - current;
                                        currentSleepToken = ThreadHelpers.RegisterWakeUp();
                                        await ThreadHelpers.SafeSleep(currentSleepToken, sleepTime);
                                        currentSleepToken = null;
                                    }
                                }
                                else {
                                    // check if we've already turned on 
                                    if (lastRule != null && lastRule.Value == currentRules[closestRuleIndex]) {
                                        // wait for the end of the rule
                                        TimeSpan sleepTime = currentRules[closestRuleIndex].EndTime - current;
                                        currentSleepToken = ThreadHelpers.RegisterWakeUp();
                                        await ThreadHelpers.SafeSleep(currentSleepToken, sleepTime);
                                        currentSleepToken = null;
                                    }
                                    else {
                                        // turn on
                                        lastRule = currentRules[closestRuleIndex];
                                        for (int i = 0; i < Constants.SCHEDULER_MAX_ATTEMPTS; i++) {
                                            bool success = await TurnOn();
                                            if (success)
                                                break;
                                            else {
                                                Console.WriteLine($"Failed to turnon, {i+1}/{Constants.SCHEDULER_MAX_ATTEMPTS}");
                                            }
                                        }
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
            if (File.Exists(ScheduleFileLoc)) {
                try {
                    string json = File.ReadAllText(ScheduleFileLoc);
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

        // Prins help and exits program
        private void printHelp() {
            Console.WriteLine("Usage: Scheduler --config {schedule.json}");
            Console.WriteLine("-----------------------------------------\n");
            Console.WriteLine("-h/--h\t\t\tPrints this help screen");
            Console.WriteLine("-config/--config\tUse this schedule config");
            Environment.Exit(0);
        }
    }
}
