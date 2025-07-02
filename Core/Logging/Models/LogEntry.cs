using System;
using System.Collections.Generic;

namespace CSTestFramework.Core.Logging.Models
{
    /// <summary>
    /// Represents a single log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// Gets or sets the timestamp of the log entry.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the log level. Only Debug is supported.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception details (if any).
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the context information.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets additional properties for structured logging.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Gets or sets the thread ID.
        /// </summary>
        public int ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the process ID.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        public LogEntry()
        {
            Timestamp = DateTime.UtcNow;
            Properties = new Dictionary<string, object>();
            ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class with specified parameters.
        /// </summary>
        /// <param name="level">The log level (only Debug is supported).</param>
        /// <param name="message">The log message.</param>
        /// <param name="context">The context information.</param>
        /// <param name="exception">The exception details.</param>
        public LogEntry(LogLevel level, string message, string context = null, Exception exception = null)
            : this()
        {
            Level = LogLevel.Debug;
            Message = message;
            Context = context;
            Exception = exception;
        }
    }
} 