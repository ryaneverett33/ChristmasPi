using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ChristmasPi.Data.Models;

namespace ChristmasPi.Data {
    public class ConfigurationManager {
        #region Singleton Methods
        private static readonly ConfigurationManager _instance = new ConfigurationManager();
        public static ConfigurationManager Instance { get { return _instance; } }
        #endregion
        public IConfiguration Configuration;
        public TreeConfiguration TreeConfiguration;
    }
}
