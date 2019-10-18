using System;

namespace ChristmasPi.Data.Interfaces {
    public interface IEasing {
        // time: the current time being evaluated
        // beginning: the starting value to ease from
        // change: the amount of change that should occur (at t=d, return beginning+change)
        // duration: how long to ease over, (t=0, return beginng. t=duration, return beginning+change)
        float EaseIn(float time, float beginning, float change, float duration);
        float EaseOut(float time, float beginning, float change, float duration);
    }
}