using System;

namespace ChristmasPi.Util.Arguments {
    [Flags]
    public enum ArgumentFlags {
        None = 0,
        CaseSensitive = 1,
        HasValue = 2,
        Required = 4,
        Reserved = 8
    }
}
