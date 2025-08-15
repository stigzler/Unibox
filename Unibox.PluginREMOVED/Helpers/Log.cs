using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unibox.Plugin.Helpers
{
    internal static class Log
    {
        private const string logPath = "Unibox.Plugin.log";

        internal static void WriteLine(string line)
        {
            File.AppendAllLines(logPath, new[] { $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {line}" }, System.Text.Encoding.UTF8);
        }
    }
}