using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace BenLib
{
    public static class Threading
    {
        public static async Task MultipleAttempts(Task task, int times = 10, int delay = 50, bool throwEx = true, Action middleAction = null, Task middleTask = null)
        {
            Exception exception = null;

            for (int i = 0; i < times; i++)
            {
                try
                {
                    await task;
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    middleAction?.Invoke();
                    if (middleTask != null) await middleTask;
                    await Task.Delay(delay);
                }
            }

            if (throwEx && exception != null) throw exception;
        }

        public static async Task<TResult> MultipleAttempts<TResult>(Task<TResult> task, int times = 10, int delay = 50, bool throwEx = true, Action middleAction = null, Task middleTask = null)
        {
            Exception exception = null;

            for (int i = 0; i < times; i++)
            {
                try { return await task; }
                catch (Exception ex)
                {
                    exception = ex;
                    middleAction?.Invoke();
                    if (middleTask != null) await middleTask;
                    await Task.Delay(delay);
                }
            }

            if (throwEx && exception != null) throw exception;
            else return default;
        }

        public static async Task MultipleAttempts(Action action, int times = 10, int delay = 50, bool throwEx = true, Action middleAction = null, Task middleTask = null)
        {
            Exception exception = null;

            for (int i = 0; i < times; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    middleAction?.Invoke();
                    if (middleTask != null) await middleTask;
                    await Task.Delay(delay);
                }
            }

            if (throwEx && exception != null) throw exception;
        }

        public static async Task<TResult> MultipleAttempts<TResult>(Func<TResult> action, int times = 10, int delay = 50, bool throwEx = true, Action middleAction = null, Task middleTask = null)
        {
            Exception exception = null;

            for (int i = 0; i < times; i++)
            {
                try { return action(); }
                catch (Exception ex)
                {
                    exception = ex;
                    middleAction?.Invoke();
                    if (middleTask != null) await middleTask;
                    await Task.Delay(delay);
                }
            }

            if (throwEx && exception != null) throw exception;
            else return default;
        }

        public static void SetInterval(Action action, int milliseconds)
        {
            var dt = new System.Timers.Timer { Interval = milliseconds, Enabled = true, AutoReset = true };
            dt.Elapsed += (sender, e) => action();

            dt.Start();
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
    }

    [Flags]
    public enum ThreadAccess
    {
        TERMINATE = 0x0001,
        SUSPEND_RESUME = 0x0002,
        GET_CONTEXT = 0x0008,
        SET_CONTEXT = 0x0010,
        SET_INFORMATION = 0x0020,
        QUERY_INFORMATION = 0x0040,
        SET_THREAD_TOKEN = 0x0080,
        IMPERSONATE = 0x0100,
        DIRECT_IMPERSONATION = 0x0200
    }

    public static partial class Extensions
    {
        /// <summary>
        /// Waits asynchronously for the process to exit.
        /// </summary>
        /// <param name="process">The process to wait for cancellation.</param>
        /// <param name="cancellationToken">A cancellation token. If invoked, the task will return immediately as canceled.</param>
        /// <returns>A Task representing waiting for the process to end.</returns>
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<object>();

            void callback(object sender, EventArgs e)
            {
                tcs.TrySetResult(null);
                process.Exited -= callback;
            }

            process.EnableRaisingEvents = true;
            process.Exited += callback;
            if (cancellationToken != default) cancellationToken.Register(() => tcs.TrySetCanceled());

            return tcs.Task;
        }

        public static TryResult TryKill(this Process process)
        {
            try
            {
                process.Kill();
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static async Task<bool> StartAsync(this Process process, CancellationToken cancellationToken = default)
        {
            bool b = false;
            await Task.Run(() => b = process.Start(), cancellationToken);
            return b;
        }

        public static TryResult TryStart(this Process process)
        {
            try { process.Start(); }
            catch (Exception ex) { return new TryResult(false, ex); }

            return new TryResult(true);
        }

        public static async Task<TryResult> TryStartAsync(this Process process, CancellationToken cancellationToken)
        {
            Exception e = null;
            try
            {
                await Task.Run(() =>
                {
                    try { process.Start(); }
                    catch (Exception ex) { e = ex; }
                }, cancellationToken);
                if (e != null) return new TryResult(false, e);
            }
            catch (Exception ex) { return new TryResult(false, ex); }

            return new TryResult(true);
        }

        public static Task WithCancellation(this Task task, CancellationToken cancellationToken) => task.IsCompleted ? task : task.ContinueWith(completedTask => { }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken) => task.IsCompleted ? task : task.ContinueWith(completedTask => completedTask.Result, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

        public static async Task WithTimeout(this Task task, int millisecondsTimeout)
        {
            if (task != await Task.WhenAny(task, Task.Delay(millisecondsTimeout))) throw new TimeoutException();
        }

        public static async Task<TResult> WithTimeout<TResult>(this Task<TResult> task, int millisecondsTimeout)
        {
            if (task == await Task.WhenAny(task, Delay<TResult>(millisecondsTimeout))) return task.Result;
            else throw new TimeoutException();
        }

        private static async Task<T> Delay<T>(int millisecondsDelay)
        {
            await Task.Delay(millisecondsDelay);
            return default;
        }

        public static Task AtLeast(this Task task, int millisecondsDelay) => Task.WhenAll(task, Task.Delay(millisecondsDelay));
        public static async Task<TResult> AtLeast<TResult>(this Task<TResult> task, int millisecondsDelay) => (await Task.WhenAll(task, Delay<TResult>(millisecondsDelay)))[0];

        public static Task AtMost(this Task task, int millisecondsDelay) => Task.WhenAny(task, Task.Delay(millisecondsDelay));
        public static async Task<TResult> AtMost<TResult>(this Task<TResult> task, int millisecondsDelay) => (await Task.WhenAny(task, Delay<TResult>(millisecondsDelay))).Result;

        public static void Suspend(this Process process)
        {
            if (process.HasExited || process.ProcessName == string.Empty) return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = Threading.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero) continue;

                Threading.SuspendThread(pOpenThread);

                Threading.CloseHandle(pOpenThread);
            }
        }

        public static TryResult TrySuspend(this Process process)
        {
            try
            {
                process.Suspend();
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }

        public static void Resume(this Process process)
        {
            if (process.HasExited || process.ProcessName == string.Empty) return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = Threading.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero) continue;

                var suspendCount = 0;

                do suspendCount = Threading.ResumeThread(pOpenThread);
                while (suspendCount > 0);

                Threading.CloseHandle(pOpenThread);
            }
        }

        public static TryResult TryResume(this Process process)
        {
            try
            {
                process.Resume();
                return new TryResult(true);
            }
            catch (Exception ex) { return new TryResult(false, ex); }
        }
    }

    /// <summary>
    /// Implementation of PauseTokenSource pattern based on the blog post: 
    /// http://blogs.msdn.com/b/pfxteam/archive/2013/01/13/cooperatively-pausing-async-methods.aspx 
    /// </summary>
    public class PauseTokenSource
    {
        private TaskCompletionSource<bool> m_paused;
        internal static readonly Task s_completedTask = Task.FromResult(true);

        public bool IsPaused
        {
            get => m_paused != null;
            set
            {
                if (value) Interlocked.CompareExchange(ref m_paused, new TaskCompletionSource<bool>(), null);
                else
                {
                    while (true)
                    {
                        var tcs = m_paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref m_paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }

        public PauseToken Token => new PauseToken(this);

        internal Task WaitWhilePausedAsync()
        {
            var cur = m_paused;
            return cur != null ? cur.Task : s_completedTask;
        }
    }

    public struct PauseToken
    {
        private readonly PauseTokenSource m_source;
        internal PauseToken(PauseTokenSource source) { m_source = source; }

        public bool IsPaused { get { return m_source != null && m_source.IsPaused; } }

        public Task WaitWhilePausedAsync() => IsPaused ? m_source.WaitWhilePausedAsync() : PauseTokenSource.s_completedTask;
    }
}
