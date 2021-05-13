using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WClipboard.Core.Utilities
{
    public enum LogLevel
    {
        Info,
        Warning,
        Critical
    }

    public class Logger
    {
        private static string GetCurrentThreadName()
        {
            var t = Thread.CurrentThread;
            if (t.Name != null) {
                return t.Name;
            } else if (t.IsThreadPoolThread) {
                return "worker";
            } else if (t.IsBackground) {
                return "background";
            } else {
                return "no name";
            }
        }

        public static void Log(LogLevel level, string message)
        {
            var msg = $"## [{DateTime.Now:yyyy-dd-MM HH:mm:ss} {level} {Thread.CurrentThread.ManagedThreadId}:{GetCurrentThreadName()}] {message}";
            Console.Error.WriteLine(msg);
#if DEBUG
            Debug.WriteLine(msg);
#endif

        }

        public static void Log(LogLevel level, Exception ex) => Log(level, GenerateExceptionString(ex));

        internal static string GenerateExceptionString(Exception ex)
        {
            return $"{ex.GetType().FullName}: {ex.Message}\r\nObject or App: {ex.Source}\r\n{ex.StackTrace}\r\n" + (ex.InnerException == null ? "" : "---INNER\r\n" + GenerateExceptionString(ex.InnerException));
        }
    }

    public interface ILogger<T>
    {
        void Log(LogLevel level, string message, [CallerMemberName] string? callerMethod = null);
        void Log(LogLevel level, Exception ex, [CallerMemberName] string? callerMethod = null);
    }

    public class Logger<T> : ILogger<T>
    {
        public void Log(LogLevel level, string message, [CallerMemberName] string? callerMethod = null)
        {
            string callerInfo = $"[{typeof(T).FullName}{(callerMethod != null ? "." + callerMethod : "")}] ";
            Logger.Log(level, callerInfo + message);
        }

        public void Log(LogLevel level, Exception ex, [CallerMemberName] string? callerMethod = null)
            => Log(level, Logger.GenerateExceptionString(ex), callerMethod);
    }
}
