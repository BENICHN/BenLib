using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace BenLib
{
    public class Threading
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
    public enum ThreadAccess : int
    {
        TERMINATE = (0x0001),
        SUSPEND_RESUME = (0x0002),
        GET_CONTEXT = (0x0008),
        SET_CONTEXT = (0x0010),
        SET_INFORMATION = (0x0020),
        QUERY_INFORMATION = (0x0040),
        SET_THREAD_TOKEN = (0x0080),
        IMPERSONATE = (0x0100),
        DIRECT_IMPERSONATION = (0x0200)
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
            EventHandler callback = null;

            callback = (sender, e) =>
            {
                tcs.TrySetResult(null);
                process.Exited -= callback;
            };

            process.EnableRaisingEvents = true;
            process.Exited += callback;
            if (cancellationToken != default)
                cancellationToken.Register(() => tcs.TrySetCanceled());

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

        public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            return task.IsCompleted // fast-path optimization
                ? task
                : task.ContinueWith(
                    completedTask => completedTask.GetAwaiter().GetResult(),
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return task.IsCompleted // fast-path optimization
                ? task
                : task.ContinueWith(
                    completedTask => completedTask.GetAwaiter().GetResult(),
                    cancellationToken,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
        }

        public static void Suspend(this Process process)
        {
            if (process.HasExited || process.ProcessName == String.Empty) return;

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
            if (process.HasExited || process.ProcessName == String.Empty) return;

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

    public class RelayCommand<T> : ICommand
    {
        #region Fields

        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
        /// </summary>
        /// <param name="execute">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        ///<returns>
        ///true if this command can be executed; otherwise, false.
        ///</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        ///<summary>
        ///Occurs when changes occur that affect whether or not the command should execute.
        ///</summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion
    }

    /// <summary>
    /// A command whose sole purpose is to relay its functionality 
    /// to other objects by invoking delegates. 
    /// The default return value for the CanExecute method is 'true'.
    /// <see cref="RaiseCanExecuteChanged"/> needs to be called whenever
    /// <see cref="CanExecute"/> is expected to return a different value.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private members
        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        private readonly Action execute;

        /// <summary>
        /// True if command is executing, false otherwise
        /// </summary>
        private readonly Func<bool> canExecute;
        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand"/> that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute) : this(execute, canExecute: null) { }

        /// <summary>
        /// Initializes a new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException("execute");
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Determines whether this <see cref="RelayCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        /// <returns>True if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) => canExecute == null ? true : canExecute();

        /// <summary>
        /// Executes the <see cref="RelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter) => execute();

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class CommandHandler : ICommand
    {
        private Action<object> _action;
        private bool _canExecute;
        public CommandHandler(Action<object> action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter) => _action(parameter);
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
