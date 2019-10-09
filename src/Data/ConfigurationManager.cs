using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using ChristmasPi.Data.Models;
using Newtonsoft.Json;

namespace ChristmasPi.Data {
    public class ConfigurationManager {
        #region Singleton Methods
        private static readonly ConfigurationManager _instance = new ConfigurationManager();
        public static ConfigurationManager Instance { get { return _instance; } }
        #endregion
        public IConfiguration Configuration;
        public TreeConfiguration TreeConfiguration;

        public void Save() {
            try {
                string json = JsonConvert.SerializeObject(TreeConfiguration, Formatting.Indented);
                if (File.Exists("configuration.json")) {
                    File.Move("configuration.json", "configuration.old.json");
                    File.WriteAllText("configuration.json", json);
                }
            }
            catch (Exception e) {
                Console.WriteLine("LOGTHIS Failed to Save tree configuration, an exception occurred");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
