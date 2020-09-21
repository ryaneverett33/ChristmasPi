using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using ChristmasPi.Data.Models;
using ChristmasPi.Data.Models.Scheduler;
using ChristmasPi.Util.Arguments;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Filters.Expressions;
using Serilog.Exceptions;

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
        public RuntimeConfiguration RuntimeConfiguration;

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

        // Actions to be performed during shutdown
        private Dictionary<string, Action> shutdownActions;

        public void LoadConfiguration(string configuration = null) {
            if (configuration == null)
                configuration = Constants.CONFIGURATION_FILE;
            if (!File.Exists(configuration)) {
                Log.ForContext("ClassName", "ConfigurationManager").Information("Tree Configuration file not found, using default values");
                StartupTreeConfig = TreeConfiguration.DefaultSettings();
            }
            else {
                try {
                    string json = File.ReadAllText(configuration);
                    StartupTreeConfig = JsonConvert.DeserializeObject<TreeConfiguration>(json);
                }
                catch (JsonSerializationException jsonerr) {
                    Log.ForContext("ClassName", "ConfigurationManager").Debug(jsonerr, "Unable to deserialize configuration file");
                    Log.ForContext("ClassName", "ConfigurationManager").Error("Unable to load configuration, file may be corrupted");
                }
                catch (Exception e) {
                    Log.ForContext("ClassName", "ConfigurationManager").Error(e, "Failed to load configuration info");
                }
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
                try {
                    string json = File.ReadAllText(schedule);
                    CurrentSchedule = JsonConvert.DeserializeObject<WeekSchedule>(json);
                }
                catch (JsonSerializationException jsonerr) {
                    Log.ForContext("ClassName", "ConfigurationManager").Debug(jsonerr, "Unable to deserialize schedule file");
                    Log.ForContext("ClassName", "ConfigurationManager").Error("Unable to load schedule, file may be corrupted");
                }
                catch (Exception e) {
                    Log.ForContext("ClassName", "ConfigurationManager").Error(e, "Exception occurred when loading schedule");
                }
            }
        }

        /// <summary>
        /// Saves the current tree configuration to a json file
        /// </summary>
        public void SaveConfiguration(string configuration = null) {
            Log.ForContext("ClassName", "ConfigurationManager").Information("Saving configuration");
            try {
                if (configuration == null)
                    configuration = Constants.CONFIGURATION_FILE;
                string json = JsonConvert.SerializeObject(CurrentTreeConfig, Formatting.Indented);
                if (File.Exists(configuration))
                    File.Move(configuration, Constants.CONFIGURATION_FILE_OLD, true);
                File.WriteAllText(configuration, json);
            }
            catch (JsonSerializationException jsonerr) {
                Log.ForContext("ClassName", "ConfigurationManager").Debug(jsonerr, "Unable to serialize configuration file");
                Log.ForContext("ClassName", "ConfigurationManager").Error("Unable to save configuration, data may be corrupted");
            }
            catch (Exception e) {
                Log.ForContext("ClassName", "ConfigurationManager").Error(e, "Failed to save tree configuration");
            }
        }

        /// <summary>
        /// Saves the current schedule to a json file
        /// </summary>
        public void SaveSchedule(string schedule = null) {
            Log.ForContext("ClassName", "ConfigurationManager").Information("Saving schedule");
            try {
                if (schedule == null)
                    schedule = Constants.SCHEDULE_FILE;
                string json = JsonConvert.SerializeObject(CurrentSchedule, Formatting.Indented);
                if (File.Exists(schedule))
                    File.Move(schedule, Constants.SCHEDULE_FILE_OLD, true);
                File.WriteAllText(schedule, json);
            }
            catch (JsonSerializationException jsonerr) {
                Log.ForContext("ClassName", "ConfigurationManager").Debug(jsonerr, "Unable to serialize schedule file");
                Log.ForContext("ClassName", "ConfigurationManager").Error("Unable to save schedule, data may be corrupted");
            }
            catch (Exception e) {
                Log.ForContext("ClassName", "ConfigurationManager").Error(e, "Failed to save schedule info");
            }
        }

        /// <summary>
        /// Loads the runtime command line arguments
        /// </summary>
        /// <param name="args">the Arguments passed to the main entry point</param>
        /// <returns>Whether or not parsing was successful (and the program should continue)</returns>
        public bool LoadRuntimeConfiguration(string[] args) {
            RuntimeConfiguration = new RuntimeConfiguration();
            ArgumentHelper helper = new ArgumentHelper(RuntimeConfiguration);
            return helper.Parse(args);
        }


        /// <summary>
        /// Handles setting up the necessary parameters for logging
        /// </summary>
        public void InitializeLogger() {
            var configuration = new LoggerConfiguration();
            var aspExp = "StartsWith(SourceContext, 'Microsoft') or SourceContext = 'Serilog.AspNetCore.RequestLoggingMiddleware'";
            if (RuntimeConfiguration.DebugLogging)
                configuration.MinimumLevel.Debug();
            else
                configuration.MinimumLevel.Information();
            configuration.Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information);
            if (!RuntimeConfiguration.NoASPLogging) {
                configuration.WriteTo.Logger(lg => lg
                    .Filter.ByIncludingOnly(aspExp)
                    .WriteTo.File(Constants.ASP_LOG_FILE,
                        rollingInterval: RollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        outputTemplate: Constants.LOG_FORMAT)
                );
            }
            configuration.Filter.ByExcluding(aspExp)
            .WriteTo.File(Constants.LOG_FILE,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                outputTemplate: Constants.LOG_FORMAT);
            if (!RuntimeConfiguration.DaemonMode || RuntimeConfiguration.DaemonLogToConsole)
                configuration.WriteTo.Console(outputTemplate: Constants.LOG_FORMAT);
            Log.Logger = configuration.CreateLogger();
            RegisterOnShutdownAction("Logger", () => {
                Log.CloseAndFlush();
            });
        }

        /// <summary>
        /// Handles setting functionality for safe shutdown
        /// </summary>
        public void InitializeShutdown() {
            shutdownActions = new Dictionary<string, Action>();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler((object sender, EventArgs args) => {
                Log.Debug("Starting shutdown procedure, actions count: {count}", shutdownActions.Count);
                if (shutdownActions.Count > 0) {
                    List<string> keys = new List<string>(shutdownActions.Keys);
                    foreach (string key in keys) {
                        Action action = shutdownActions[key];
                        Log.Debug("Performing shutdown action for {key}", key);
                        try {
                            action();
                        }
                        catch (Exception e) {
                            Log.Debug(e, "Shutdown action failed for {key}", key);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Register Action to be run during shutdown
        /// </summary>
        /// <param name="name">The name of the action to avoid duplicates</param>
        /// <param name="action">The action to be performed</param>
        public void RegisterOnShutdownAction(string name, Action action) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (action == null)
                throw new ArgumentNullException("action");
            if (shutdownActions.ContainsKey(name))
                throw new ArgumentException("Action already registered for name");
            shutdownActions.Add(name, action);
        }
    }
}
