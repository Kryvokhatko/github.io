using System;

namespace CSTestFramework.Core.Logging.Interfaces
{
    /// <summary>
    /// Defines the core logging interface for the test framework (Debug only).
    /// </summary>
    public interface ILogger
    {
        void Debug(string message, params object[] args);
        void Debug(Exception exception, string message, params object[] args);
        ILogger ForContext(string context);
        ILogger ForContext(string propertyName, object propertyValue);
    }
} 