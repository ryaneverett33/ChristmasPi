using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Scheduler;
using ChristmasPi.Util.Arguments;
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
        /// Debug configuration specified at runtime
        /// </summary>
        public DebugConfiguration DebugConfiguration;

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

        public void LoadConfiguration(string configuration = null) {
            if (configuration == null)
                configuration = Constants.CONFIGURATION_FILE;
            if (!File.Exists(configuration)) {
                Console.WriteLine("LOGTHIS Tree Configuration file not found, using default configuration values");
                StartupTreeConfig = TreeConfiguration.DefaultSettings();
            }
            else {
                string json = File.ReadAllText(configuration);
                StartupTreeConfig = JsonConvert.DeserializeObject<TreeConfiguration>(json);
            }
            CurrentTreeConfig = StartupTreeConfig;
        }

        public void LoadSchedule(string schedule = null) {
            if (schedule == null)
                schedule = Constants.SCHEDULE_FILE;
            if (!File.Exists(schedule)) {
                CurrentSchedule = WeekSchedule.DefaultSchedule();
            }
            else {
                string json = File.ReadAllText(schedule);
                CurrentSchedule = JsonConvert.DeserializeObject<WeekSchedule>(json);
            }
        }

        /// <summary>
        /// Saves the current tree configuration to a json file
        /// </summary>
        public void SaveConfiguration(string configuration = null) {
            Console.WriteLine("Saving configuration");
            try {
                if (configuration == null)
                    configuration = Constants.CONFIGURATION_FILE;
                string json = JsonConvert.SerializeObject(CurrentTreeConfig, Formatting.Indented);
                if (File.Exists(configuration))
                    File.Move(configuration, Constants.CONFIGURATION_FILE_OLD, true);
                File.WriteAllText(configuration, json);
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
        public void SaveSchedule(string schedule = null) {
            Console.WriteLine("Saving schedule");
            try {
                if (schedule == null)
                    schedule = Constants.SCHEDULE_FILE;
                string json = JsonConvert.SerializeObject(CurrentSchedule, Formatting.Indented);
                if (File.Exists(schedule))
                    File.Move(schedule, Constants.SCHEDULE_FILE_OLD, true);
                File.WriteAllText(schedule, json);
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS Failed to save schedule info, an exception occurred");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Loads the runtime command line arguments
        /// </summary>
        /// <param name="args">the Arguments passed to the main entry point</param>
        /// <returns>Whether or not parsing was successful (and the program should continue)</returns>
        public bool LoadDebugConfiguration(string[] args) {
            DebugConfiguration = new DebugConfiguration();
            ArgumentHelper helper = new ArgumentHelper(DebugConfiguration);
            return helper.Parse(args);
        }
    }
}
