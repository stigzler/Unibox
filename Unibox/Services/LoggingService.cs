using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Unibox.Services
{
    internal class LoggingService
    {
        public string LogFilepath { get; set; } = "Unibox.Plugin.log";
        private readonly object _lock = new();

        public LoggingService()
        {
            StartLog();
        }

        public void StartLog()
        {
            lock (_lock)
            {
                using var stream = new FileStream(LogFilepath, FileMode.Create, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(stream, Encoding.UTF8);
                writer.WriteLine($"Logging started at {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            }
        }

        public void WriteLine(string message)
        {
            var logEntry = $"[{DateTime.Now:dd-MM HH:mm:ss.fff}] {message}";
            lock (_lock)
            {
                using var stream = new FileStream(LogFilepath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(stream, Encoding.UTF8);
                writer.WriteLine(logEntry);
            }
        }
    }
}