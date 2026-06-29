using System;
using System.IO;
using PSAutomationToolkit.Models;

namespace PSAutomationToolkit.Services
{
    public class LoggingService
    {
        private readonly string _logPath;

        public LoggingService()
        {
            var logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PSAutomationToolkit", "Logs");
            Directory.CreateDirectory(logDir);
            _logPath = Path.Combine(logDir, $"PSToolkit_{DateTime.Now:yyyyMMdd}.log");
        }

        public void Log(string action, string details, string level = "INFO")
        {
            try
            {
                var entry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Action    = action,
                    Details   = details,
                    Level     = level
                };

                var line = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Level,-5}] {entry.Action} | {entry.Details}";
                File.AppendAllText(_logPath, line + Environment.NewLine);
            }
            catch { /* Silently fail logging */ }
        }

        public string GetLogPath() => _logPath;

        public string[] GetRecentLogs(int lines = 100)
        {
            try
            {
                if (!File.Exists(_logPath)) return Array.Empty<string>();
                var all = File.ReadAllLines(_logPath);
                var start = Math.Max(0, all.Length - lines);
                return all[start..];
            }
            catch { return Array.Empty<string>(); }
        }
    }
}
