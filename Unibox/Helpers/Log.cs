using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Helpers
{
    internal static class Log
    {
        private const string logPath = @"Unibox.log";

        internal static void WriteLine(string line)
        {
            //File.AppendAllLines(logPath, new[] { $"{DateTime.Now:HH:mm:ss} - {line}" }, System.Text.Encoding.UTF8);
        }

        internal static void StartLog()
        {
            // File.WriteAllText(logPath, $"Log started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n", System.Text.Encoding.UTF8);
        }
    }
}