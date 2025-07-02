using System;
using CSTestFramework.Core.Logging.Models;
using ILogger = CSTestFramework.Core.Logging.Interfaces.ILogger;

namespace CSTestFramework.Core.Logging.Implementations
{
    /// <summary>
    /// Serilog-based implementation of the ILogger interface.
    /// </summary>
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger _serilogLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogLogger"/> class.
        /// </summary>
        /// <param name="serilogLogger">The underlying Serilog logger instance.</param>
        public SerilogLogger(Serilog.ILogger serilogLogger)
        {
            _serilogLogger = serilogLogger ?? throw new ArgumentNullException(nameof(serilogLogger));
        }

        /// <inheritdoc/>
        public void Debug(string message, params object[] args)
        {
            _serilogLogger.Debug(message, args);
        }

        /// <inheritdoc/>
        public void Debug(Exception exception, string message, params object[] args)
        {
            _serilogLogger.Debug(exception, message, args);
        }

        /// <inheritdoc/>
        public ILogger ForContext(string context)
        {
            var contextLogger = _serilogLogger.ForContext("Context", context);
            return new SerilogLogger(contextLogger);
        }

        /// <inheritdoc/>
        public ILogger ForContext(string propertyName, object propertyValue)
        {
            var contextLogger = _serilogLogger.ForContext(propertyName, propertyValue);
            return new SerilogLogger(contextLogger);
        }
    }
} 