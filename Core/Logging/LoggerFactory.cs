using System;
using System.IO;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Logging.Implementations;
using Serilog;
using Serilog.Events;
using ILogger = CSTestFramework.Core.Logging.Interfaces.ILogger;
using CSTestFramework.Core.Configuration.Models;

namespace CSTestFramework.Core.Logging
{
    /// <summary>
    /// Factory class for creating and configuring loggers.
    /// </summary>
    public static class LoggerFactory
    {
        private static Serilog.ILogger _rootLogger;
        private static bool _isInitialized;

        /// <summary>
        /// Initializes the logging system with the specified configuration.
        /// </summary>
        /// <param name="loggingConfiguration">The logging configuration.</param>
        public static void Initialize(ILoggingConfiguration loggingConfiguration)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("LoggerFactory has already been initialized.");
            }

            if (loggingConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggingConfiguration));
            }

            var loggerConfiguration = new LoggerConfiguration();

            // Set minimum log level
            if (Enum.TryParse<LogEventLevel>(loggingConfiguration.MinimumLevel, true, out var minLevel))
            {
                loggerConfiguration.MinimumLevel.Is(minLevel);
            }

            // Configure console logging if enabled
            if (loggingConfiguration.EnableConsoleLogging)
            {
                loggerConfiguration.WriteTo.Console(
                    outputTemplate: loggingConfiguration.LogFormat ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            // Configure file logging if enabled
            if (loggingConfiguration.EnableFileLogging)
            {
                var logFilePath = loggingConfiguration.LogFilePath ?? "logs";
                var logFileTemplate = loggingConfiguration.LogFileTemplate ?? "log-.txt";

                // Ensure log directory exists
                Directory.CreateDirectory(logFilePath);

                var fullLogPath = Path.Combine(logFilePath, logFileTemplate);

                loggerConfiguration.WriteTo.File(
                    path: fullLogPath,
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: loggingConfiguration.MaxFileSize,
                    retainedFileCountLimit: loggingConfiguration.MaxFileCount,
                    outputTemplate: loggingConfiguration.LogFormat ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            // Add structured logging properties if enabled
            if (loggingConfiguration.IncludeStructuredProperties)
            {
                loggerConfiguration.Enrich.FromLogContext();
            }

            _rootLogger = loggerConfiguration.CreateLogger();
            _isInitialized = true;
        }

        /// <summary>
        /// Creates a logger instance.
        /// </summary>
        /// <returns>A logger instance.</returns>
        public static ILogger CreateLogger()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("LoggerFactory must be initialized before creating loggers.");
            }

            return new SerilogLogger(_rootLogger);
        }

        /// <summary>
        /// Creates a logger instance for a specific context.
        /// </summary>
        /// <param name="context">The context name.</param>
        /// <returns>A logger instance for the specified context.</returns>
        public static ILogger CreateLogger(string context)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("LoggerFactory must be initialized before creating loggers.");
            }

            var contextLogger = _rootLogger.ForContext("Context", context);
            return new SerilogLogger(contextLogger);
        }

        /// <summary>
        /// Creates a logger instance for a specific type.
        /// </summary>
        /// <typeparam name="T">The type to create a logger for.</typeparam>
        /// <returns>A logger instance for the specified type.</returns>
        public static ILogger CreateLogger<T>()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("LoggerFactory must be initialized before creating loggers.");
            }

            var contextLogger = _rootLogger.ForContext<T>();
            return new SerilogLogger(contextLogger);
        }

        /// <summary>
        /// Disposes the logging system and flushes any pending log entries.
        /// </summary>
        public static void Dispose()
        {
            if (_isInitialized && _rootLogger != null)
            {
                (_rootLogger as IDisposable)?.Dispose();
                _rootLogger = null;
                _isInitialized = false;
            }
        }
    }
} 