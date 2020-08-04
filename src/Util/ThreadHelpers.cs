using ChristmasPi.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

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
            if (sleep.TotalMilliseconds <= 0)
                return true;
            try {
                await Task.Delay(sleep, token.Token);
                return true;
            }
            catch (TaskCanceledException) {
                return false;
            }
            catch (Exception e) {
                Log.ForContext("ClassName", "ThreadHelpers").Error(e, "SafeSleep encountered an exception");
                return false;
            }
        }
        public static void UnSafeSleep(TimeSpan sleep) {
            Thread.Sleep(sleep);
        }
        public static void UnSafeSleep(int ms) {
            UnSafeSleep(new TimeSpan(0, 0, 0, 0, ms));
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
        /// Calculates the remaining ms in a second needed for synchronization
        /// </summary>
        /// <param name="fps">The current running fps</param>
        /// <param name="waitTime">The calculated waitTime for a thread</param>
        /// <returns>Time in ms to wait</returns>
        /// <seealso cref="CalculateWaitTime(int)"/>
        public static int CalculateSyncTime(int fps, int waitTime) {
            return Math.Clamp(1000 - (fps * waitTime), 0, 10);
        }

        /// <summary>
        /// Calculates the time a thread should sleep to maintain current fps
        /// </summary>
        /// <param name="fps">The desired fps</param>
        /// <returns>Time in ms to wait</returns>
        public static int CalculateWaitTime(int fps) {
            if (fps <= 0 || fps > Constants.FPS_MAX) {
                fps = Constants.FPS_DEFAULT;
                Log.ForContext("ClassName", "ThreadHelpers").Information("Invalid fps value: {fps}. Using default fps: {default}", fps, Constants.FPS_MAX);
            }
            float value = (1f / (float)fps) * 1000f;
            return (int)Math.Floor(value);
        }
    }
}
