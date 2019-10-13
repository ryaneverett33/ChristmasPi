using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Animation.Interfaces {
    public interface ILegacyAnimation : IAnimatable {
        bool doStopExecuting { get; set; }
    }
}
