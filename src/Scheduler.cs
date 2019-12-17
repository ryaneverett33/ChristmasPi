using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ChristmasPi.Data;
using ChristmasPi.Data.Models.Scheduler;
using ChristmasPi.Util;
using ChristmasPi.Data.Extensions;
using System.Runtime.Loader;

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
        private bool unloading = false;

        public Scheduler(string[] args) {
            for (int i = 0; i < args.Length; i++) {
                if (args[i].Equals("--config", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-config", StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 < args.Length) {
                        ScheduleFileLoc = args[i + 1];
                        i++;
                        continue;
                    }
                    Console.WriteLine("Invalid config argument");
                    printHelp();
                }
                else if (args[i].Equals("--url", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-url", StringComparison.CurrentCultureIgnoreCase)) {
                    if (i + 1 < args.Length) {
                        apiURL = args[i + 1];
                        i++;
                        continue;
                    }
                    Console.WriteLine("Invalid config argument");
                    printHelp();
                }
                else if (args[i].Equals("--h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("-h", StringComparison.CurrentCultureIgnoreCase) ||
                    args[i].Equals("help", StringComparison.CurrentCultureIgnoreCase))
                    printHelp();
            }
            if (ScheduleFileLoc == null)
                ScheduleFileLoc = Constants.SCHEDULE_FILE;
            if (apiURL == null)
                apiURL = $"http://localhost:{Constants.PORT}/api";
            ScheduleFileLoc = Path.GetFullPath(ScheduleFileLoc);
            watcher = new FileSystemWatcher(Path.GetDirectoryName(ScheduleFileLoc));
            watcher.Filter = Path.GetFileName(ScheduleFileLoc);
            watcher.Changed += FileChanged;
            watcher.Created += FileChanged;
            watcher.Deleted += FileChanged;
            watcher.EnableRaisingEvents = true;
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler; //register sigterm event handler. Don't forget to import System.Runtime.Loader!
            Console.CancelKeyPress += CancelHandler; //register sigint event 
            Running = true;
            Scheduling = false;
            httpClient = new HttpClient();
        }

        public void Run() {
            while (Running) {
                // get schedule info
                if (loadSchedule()) {
                    Scheduling = true;
                    while(Scheduling) {
                        TimeSlot[] currentRules = getCurrentSchedule().GetRules();
                        if (currentRules.Length == 0) {
                            if (lastRule != null) {
                                Console.WriteLine("Just exited a rule");
                                TurnOff();
                            }
                            Console.WriteLine("No rules for the day, sleeping");
                            // long sleep
                            currentSleepToken = ThreadHelpers.RegisterWakeUp();
                            ThreadHelpers.SafeSleep(currentSleepToken, Constants.SCHEDULER_LONG_SLEEP).Wait();            // ignore the result but wakeup if need be
                            //ThreadHelpers.UnSafeSleep(Constants.SCHEDULER_LONG_SLEEP);
                            currentSleepToken = null;
                        }
                        else {
                            // get current rule
                            int closestRuleIndex = getClosestRule(currentRules);
                            if (closestRuleIndex == -1) {
                                if (lastRule != null) {
                                    Console.WriteLine("Just exited a rule");
                                    TurnOff();
                                }
                                Console.WriteLine("No more rules, sleeping for {0}", Constants.SCHEDULER_LONG_SLEEP.ToString());
                                // no more rules for the current day, go to sleep
                                currentSleepToken = ThreadHelpers.RegisterWakeUp();
                                ThreadHelpers.SafeSleep(currentSleepToken, Constants.SCHEDULER_LONG_SLEEP).Wait();            // ignore the result but wakeup if need be
                                //ThreadHelpers.UnSafeSleep(Constants.SCHEDULER_LONG_SLEEP);
                                currentSleepToken = null;
                            }
                            else {
                                // check if we're in the rule or need to wait for the rule
                                DateTime current = DateTime.Now.ZeroOut();
                                if (current < currentRules[closestRuleIndex].StartTime) {
                                    // check if we just left a rule
                                    if (lastRule != null) {
                                        // turn off
                                        Console.WriteLine("Just exited a rule");
                                        TurnOff();
                                    }
                                    else {
                                        // wait for rule to start
                                        TimeSpan sleepTime = currentRules[closestRuleIndex].StartTime - current;
                                        Console.WriteLine("Waiting for next rule to start, sleeping for {0}", sleepTime.ToString());
                                        currentSleepToken = ThreadHelpers.RegisterWakeUp();
                                        ThreadHelpers.SafeSleep(currentSleepToken, sleepTime).Wait();
                                        //ThreadHelpers.UnSafeSleep(sleepTime);
                                        currentSleepToken = null;
                                    }
                                }
                                else {
                                    // check if we've already turned on 
                                    if (lastRule != null && lastRule.Value == currentRules[closestRuleIndex]) {
                                        // wait for the end of the rule
                                        TimeSpan sleepTime = currentRules[closestRuleIndex].EndTime - current;
                                        Console.WriteLine("Waiting for current rule to end, sleeping for {0}", sleepTime.ToString());
                                        currentSleepToken = ThreadHelpers.RegisterWakeUp();
                                        ThreadHelpers.SafeSleep(currentSleepToken, sleepTime).Wait();
                                        //ThreadHelpers.UnSafeSleep(sleepTime);
                                        currentSleepToken = null;
                                    }
                                    else {
                                        Console.WriteLine("Just entered a rule");
                                        // turn on
                                        lastRule = currentRules[closestRuleIndex];
                                        TurnOn();
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    // sleep for a while and then try again
                    currentSleepToken = ThreadHelpers.RegisterWakeUp();
                    ThreadHelpers.SafeSleep(currentSleepToken, Constants.SCHEDULER_ERR_SLEEP).Wait();            // ignore the result but wakeup if need be
                    //ThreadHelpers.UnSafeSleep(Constants.SCHEDULER_ERR_SLEEP);
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

        private void SigTermEventHandler(AssemblyLoadContext obj) {
            // Per https://logankpaschke.com/linux/systemd/dotnet/systemd-dotnet-1/#
            // close application
            if (!unloading) {
                Console.WriteLine("Recieved SIGTERM, will unload");
                unload();
            }
        }

        private void CancelHandler(object sender, ConsoleCancelEventArgs e) {
            if (!unloading) {
                Console.WriteLine("Recieved SIGINT, will unload");
                unload();
            }
        }

        private void TurnOff() {
            lastRule = null;
            for (int i = 0; i < Constants.SCHEDULER_MAX_ATTEMPTS; i++) {
                bool success = TurnOffAsync().Result;
                if (success) {
                    Console.WriteLine($"[{DateTime.Now.ToShortDateString()}-{DateTime.Now.ToShortTimeString()}]Successfully turned off");
                    break;
                }
                else {
                    Console.WriteLine($"Failed to turnoff, {i + 1}/{Constants.SCHEDULER_MAX_ATTEMPTS}");
                }
            }
        }
        private void TurnOn() {
            for (int i = 0; i < Constants.SCHEDULER_MAX_ATTEMPTS; i++) {
                try {
                    bool success = TurnOnAsync().Result;
                    if (success) {
                        Console.WriteLine($"[{DateTime.Now.ToShortDateString()}-{DateTime.Now.ToShortTimeString()}]Successfully turned on");
                        break;
                    }
                    else {
                        Console.WriteLine($"Failed to turnon, {i + 1}/{Constants.SCHEDULER_MAX_ATTEMPTS}");
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }

        // Turns on the christmas tree
        private async Task<bool> TurnOnAsync() {
            try {
                HttpResponseMessage response = await httpClient.PostAsync($"{apiURL}/power/on", null);
                // if ((int)response.StatusCode != 200)
                //    Console.WriteLine($"Failed to turn on, status code: {(int)response.StatusCode}");
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (HttpRequestException e) {
                Console.WriteLine($"Failed to execute turnon command, exception: {e.Message}");
            }
            catch (Exception e) {
                Console.WriteLine($"Exception occured turning on, {e}");
                return false;
            }
            return false;
        }
        
        // Turns off the christmas tree
        private async Task<bool> TurnOffAsync() {
            try {
                HttpResponseMessage response = await httpClient.PostAsync($"{apiURL}/power/off", null);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (HttpRequestException e) {
                Console.WriteLine($"Failed to execute turnon command, exception: {e.Message}");
            }
            catch (Exception e) {
                Console.WriteLine($"Exception occured turning off, {e}");
                return false;
            }
            return false;
        }

        // loads the schedule if the schedule file exists
        private bool loadSchedule() {
            if (File.Exists(ScheduleFileLoc)) {
                try {
                    string json = File.ReadAllText(ScheduleFileLoc);
                    schedule = JsonConvert.DeserializeObject<WeekSchedule>(json);
                    //Console.WriteLine("Loaded schedule");
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
                else if (current > rule.StartTime && current < rule.EndTime)
                    return i;
            }
            return -1;
        }

        // Clean up and exit
        private void unload() {
            if (!unloading) {
                unloading = true;
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                Scheduling = false;
                Running = false;
                if (currentSleepToken != null)
                    ThreadHelpers.WakeUpThread(currentSleepToken);
                httpClient.Dispose();
                Environment.Exit(0);
            }
        }

        // Prints help and exits program
        private void printHelp() {
            Console.WriteLine("Usage: Scheduler --config {schedule.json}");
            Console.WriteLine("-----------------------------------------\n");
            Console.WriteLine("-h/--h\t\t\tPrints this help screen");
            Console.WriteLine("-config/--config\tUse this schedule config");
            Console.WriteLine("-url/--url\tUse this api url");
            Environment.Exit(0);
        }
    }
}
