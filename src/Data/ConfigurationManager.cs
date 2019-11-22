using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using ChristmasPi.Data.Models;
using ChristmasPi.Scheduler.Models;
using Newtonsoft.Json;

namespace ChristmasPi.Data {
    public class ConfigurationManager {
        #region Singleton Methods
        private static readonly ConfigurationManager _instance = new ConfigurationManager();
        public static ConfigurationManager Instance { get { return _instance; } }
        #endregion

        /// <summary>
        /// ASP.NET Configuration data
        /// </summary>
        public IConfiguration Configuration;

        /// <summary>
        /// Tree Configuration loaded at startup
        /// </summary>
        public TreeConfiguration StartupTreeConfig;

        /// <summary>
        /// Tree Configuration currently being used, saved or unsaved
        /// </summary>
        public TreeConfiguration CurrentTreeConfig;

        /// <summary>
        /// The current scheduling information being used
        /// </summary>
        public WeekSchedule CurrentSchedule;

        /// <summary>
        /// Saves the current tree configuration to a json file
        /// </summary>
        public void Save() {
            Console.WriteLine("Saving configuration");
            try {
                string json = JsonConvert.SerializeObject(CurrentTreeConfig, Formatting.Indented);
                if (File.Exists(Constants.CONFIGURATION_FILE))
                    File.Move(Constants.CONFIGURATION_FILE, Constants.CONFIGURATION_FILE_OLD);
                File.WriteAllText(Constants.CONFIGURATION_FILE, json);
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS Failed to save tree configuration, an exception occurred");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Saves the current schedule to a json file
        /// </summary>
        public void SaveSchedule() {
            Console.WriteLine("Saving schedule");
            try {
                string json = JsonConvert.SerializeObject(CurrentSchedule, Formatting.Indented);
                if (File.Exists(Constants.SCHEDULE_FILE))
                    File.Move(Constants.SCHEDULE_FILE, Constants.SCHEDULE_FILE_OLD, true);
                File.WriteAllText(Constants.SCHEDULE_FILE, json);
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS Failed to save schedule info, an exception occurred");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
