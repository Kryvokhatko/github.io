using System;
using CSTestFramework.Core.Logging.Interfaces;
using ILoggingConfiguration = CSTestFramework.Core.Logging.Interfaces.ILoggingConfiguration;

namespace CSTestFramework.Core.Logging
{
    /// <summary>
    /// Helper class for setting up logging in test scenarios.
    /// </summary>
    public static class TestLoggerHelper
    {
        /// <summary>
        /// Initializes the logging system for tests using the provided configuration.
        /// </summary>
        /// <param name="loggingConfiguration">The logging configuration.</param>
        public static void InitializeLogging(ILoggingConfiguration loggingConfiguration)
        {
            LoggerFactory.Initialize(loggingConfiguration);
        }

        /// <summary>
        /// Creates a logger for a test class.
        /// </summary>
        /// <typeparam name="T">The test class type.</typeparam>
        /// <returns>A logger instance for the test class.</returns>
        public static ILogger CreateTestLogger<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// Creates a logger for a specific test context.
        /// </summary>
        /// <param name="testName">The name of the test.</param>
        /// <returns>A logger instance for the test.</returns>
        public static ILogger CreateTestLogger(string testName)
        {
            return LoggerFactory.CreateLogger(testName);
        }

        /// <summary>
        /// Logs test execution information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="testName">The name of the test.</param>
        /// <param name="testMethod">The test method name.</param>
        /// <param name="parameters">Optional test parameters.</param>
        public static void LogTestStart(ILogger logger, string testName, string testMethod, object parameters = null)
        {
            logger.Debug("Starting test: {TestName} - Method: {TestMethod}", testName, testMethod);
            
            if (parameters != null)
            {
                logger.Debug("Test parameters: {@Parameters}", parameters);
            }
        }

        /// <summary>
        /// Logs test completion information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="testName">The name of the test.</param>
        /// <param name="duration">The test duration.</param>
        /// <param name="status">The test status (Passed/Failed).</param>
        public static void LogTestEnd(ILogger logger, string testName, TimeSpan duration, string status)
        {
            logger.Debug("Test completed: {TestName} - Status: {Status} - Duration: {Duration:g}", 
                testName, status, duration);
        }

        /// <summary>
        /// Logs test failure information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="testName">The name of the test.</param>
        /// <param name="exception">The exception that caused the failure.</param>
        public static void LogTestFailure(ILogger logger, string testName, Exception exception)
        {
            logger.Debug(exception, "Test failed: {TestName}", testName);
        }

        /// <summary>
        /// Logs step execution information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="stepName">The name of the step.</param>
        /// <param name="stepDescription">The step description.</param>
        public static void LogStep(ILogger logger, string stepName, string stepDescription)
        {
            logger.Debug("Executing step: {StepName} - {StepDescription}", stepName, stepDescription);
        }

        /// <summary>
        /// Logs step completion information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="stepName">The name of the step.</param>
        /// <param name="duration">The step duration.</param>
        public static void LogStepCompleted(ILogger logger, string stepName, TimeSpan duration)
        {
            logger.Debug("Step completed: {StepName} - Duration: {Duration:g}", stepName, duration);
        }

        /// <summary>
        /// Logs step failure information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="stepName">The name of the step.</param>
        /// <param name="exception">The exception that caused the failure.</param>
        public static void LogStepFailure(ILogger logger, string stepName, Exception exception)
        {
            logger.Debug(exception, "Step failed: {StepName}", stepName);
        }

        /// <summary>
        /// Disposes the logging system.
        /// </summary>
        public static void DisposeLogging()
        {
            LoggerFactory.Dispose();
        }
    }
} 