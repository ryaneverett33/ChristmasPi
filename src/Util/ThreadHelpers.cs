﻿using ChristmasPi.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChristmasPi.Util {
    public class ThreadHelpers {
        /// <summary>
        /// Registers the thread to be able to wake up
        /// </summary>
        /// <returns>A new cancellation token</returns>
        public static CancellationTokenSource RegisterWakeUp() {
            return new CancellationTokenSource();
        }

        /// <summary>
        /// A safe alternative to Thread.Sleep allowing the thread to be woken up
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="sleep">How long to sleep for</param>
        /// <returns>True if sleep was successful, false if woken up</returns>
        /// <remarks>A cancellation token should be registered first before using</remarks>
        /// <see cref="RegisterWakeUp"/>
        public static async Task<bool> SafeSleep(CancellationTokenSource token, TimeSpan sleep) {
            try {
                await Task.Delay(sleep, token.Token);
                return true;
            }
            catch (TaskCanceledException) {
                return false;
            }
        }

        /// <summary>
        /// A safe alternative to Thread.Sleep allowing the thread to be woken up
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="sleepms">How long to sleep for (in ms)</param>
        /// <returns>True if sleep was successful, false if woken up</returns>
        public static async Task<bool> SafeSleep(CancellationTokenSource token, int sleepms) {
            return await SafeSleep(token, new TimeSpan(0, 0, 0, 0, sleepms));
        }

        /// <summary>
        /// Wakes up a thread/cancels its sleep
        /// </summary>
        /// <param name="token">Cancellation token</param>
        public static void WakeUpThread(CancellationTokenSource token) {
            token.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fps"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public static int CalculateSyncTime(int fps, int waitTime) {
            return Math.Clamp(1000 - (fps * waitTime), 0, 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fps"></param>
        /// <returns></returns>
        public static int CalculateWaitTime(int fps) {
            if (fps <= 0 || fps > Constants.FPS_MAX) {
                fps = Constants.FPS_DEFAULT;
                Console.WriteLine("LOGTHIS Invalid fps value. Using default fps");
            }
            float value = (1f / (float)fps) * 1000f;
            return (int)Math.Floor(value);
        }
    }
}
