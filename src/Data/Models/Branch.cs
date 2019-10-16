using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    public class Branch {
        
        public int start { get; set; }

        public int end { get; set; }

        public int LightCount => (end - start) + 1; // offset for zero-based indexing

        /// <summary>
        /// Returns the default settings for the setup process
        /// </summary>
        public static Branch DefaultSettings() {
            Branch branch = new Branch();
            branch.start = 0;
            branch.end = 1;
            return branch;
        }
    }
}
