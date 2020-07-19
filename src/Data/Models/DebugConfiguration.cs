﻿using System;
using ChristmasPi.Util.Arguments;

namespace ChristmasPi.Data.Models {
    public class DebugConfiguration {

        [HelpSection("Debug")]
        [Argument("tr", "Allows the test renderer to be selected during setup", true)]
        public bool AllowTestRenderer;

        [HelpSection("Debug")]
        [Argument("ignorepriv", "Ignore admin privilege requirement", true)]
        public bool IgnorePrivileges;

        public DebugConfiguration() {
            AllowTestRenderer = false;
            IgnorePrivileges = false;
        }
    }
}
