namespace CSTestFramework.Core.Logging.Interfaces
{
    /// <summary>
    /// Defines the logging configuration interface.
    /// </summary>
    public interface ILoggingConfiguration
    {
        /// <summary>
        /// Gets the minimum log level.
        /// </summary>
        string MinimumLevel { get; }

        /// <summary>
        /// Gets the log file path.
        /// </summary>
        string LogFilePath { get; }

        /// <summary>
        /// Gets the log file template.
        /// </summary>
        string LogFileTemplate { get; }

        /// <summary>
        /// Gets the maximum log file size in bytes.
        /// </summary>
        long MaxFileSize { get; }

        /// <summary>
        /// Gets the maximum number of log files to retain.
        /// </summary>
        int MaxFileCount { get; }

        /// <summary>
        /// Gets whether to enable console logging.
        /// </summary>
        bool EnableConsoleLogging { get; }

        /// <summary>
        /// Gets whether to enable file logging.
        /// </summary>
        bool EnableFileLogging { get; }

        /// <summary>
        /// Gets the log format template.
        /// </summary>
        string LogFormat { get; }

        /// <summary>
        /// Gets whether to include structured logging properties.
        /// </summary>
        bool IncludeStructuredProperties { get; }
    }
} 