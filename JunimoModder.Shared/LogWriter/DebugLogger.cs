using System;
using System.Collections.Generic;
using System.IO;

namespace JunimoModder.Shared.LogWriter
{
    /// <summary>
    /// Logs errors, warnings, and info for debugging purposes.
    /// </summary>
    public static class DebugLogger
    {
        // ===================== Fields =====================

        /// <summary>
        /// Stores all log entries.
        /// </summary>
        private static readonly List<string> _log = new();

        // ===================== Methods =====================

        /// <summary>
        /// Logs an info message.
        /// </summary>
        public static void Info(string message) => Write("INFO", message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public static void Warn(string message) => Write("WARN", message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public static void Error(string message) => Write("ERROR", message);

        /// <summary>
        /// Writes a log entry with the given level and message.
        /// </summary>
        /// <param name="level">Log level (INFO, WARN, ERROR).</param>
        /// <param name="message">The log message.</param>
        private static void Write(string level, string message)
        {
            var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            _log.Add(entry);
            // TODO: Optionally write to file
        }

        /// <summary>
        /// Returns the entire log as an enumerable.
        /// </summary>
        public static IEnumerable<string> GetLog() => _log;
    }
}
