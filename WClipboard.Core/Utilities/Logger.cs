using System;
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

        public static void Log(LogLevel level, string message)
        {
            Console.Error.WriteLine($"## [{DateTime.Now:yyyy-dd-MM HH:mm:ss} {level} {Thread.CurrentThread.Name}] {message}");
        }

        public static void Log(LogLevel level, Exception ex) => Log(level, GenerateExceptionString(ex));

        private static string GenerateExceptionString(Exception ex)
        {
            return $"{ex.GetType().FullName}: {ex.Message}\r\nObject or App: {ex.Source}\r\n{ex.StackTrace}\r\n" + (ex.InnerException == null ? "" : "---INNER\r\n" + GenerateExceptionString(ex.InnerException));
        }
    }
}
