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
    }
}
