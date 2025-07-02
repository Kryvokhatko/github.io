using System;
using System.Diagnostics;
using NUnit.Framework;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Configuration;
using CSTestFramework.Core.Logging;
using CSTestFramework.Core.Configuration.Models;
using OpenQA.Selenium;
using System.Text;
using Newtonsoft.Json;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Represents the severity level of a test.
    /// </summary>
    public enum SeverityLevel
    {
        Blocker,
        Critical,
        High,
        Normal,
        Medium,
        Minor,
        Trivial
    }

    /// <summary>
    /// Base class for all tests with integrated Allure reporting and automatic screenshot functionality.
    /// </summary>
    public abstract class TestBase
    {
        protected ILogger Logger { get; private set; }
        protected AllureTestContext AllureContext { get; private set; }
        protected Stopwatch TestStopwatch { get; private set; }
        protected ScreenshotService ScreenshotService { get; private set; }

        /// <summary>
        /// Gets or sets the test severity level.
        /// </summary>
        protected virtual SeverityLevel TestSeverity => SeverityLevel.Normal;

        /// <summary>
        /// Gets or sets the test category.
        /// </summary>
        protected virtual string TestCategory => "General";

        /// <summary>
        /// Gets or sets the test description.
        /// </summary>
        protected virtual string TestDescription => null;

        /// <summary>
        /// Gets or sets the WebDriver instance for UI tests.
        /// Override this property in UI test classes to provide WebDriver access.
        /// </summary>
        protected virtual IWebDriver WebDriver => null;

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            try
            {
                var testSuiteName = GetType().Name;
                Logger = TestLoggerHelper.CreateTestLogger(testSuiteName);
                Logger.Debug("Test suite setup started: {TestSuite}", testSuiteName);

                AllureContext = AllureTestContext.Current;

                var loggingSettings = ConfigurationManager.Instance.AppSettings.Logging;
                ScreenshotService = new ScreenshotService(loggingSettings);
                Logger.Debug("Screenshot service initialized with mode: {ScreenshotMode}", loggingSettings.ScreenshotMode);

                TestStopwatch = new Stopwatch();

                Logger.Debug("Test suite setup completed: {TestSuite}", GetType().Name);
            }
            catch (Exception ex)
            {
                Logger?.Debug(ex, "Test suite setup failed: {TestSuite}", GetType().Name);
                throw;
            }
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            try
            {
                TestStopwatch?.Stop();
                Logger?.Debug("Test suite teardown completed: {TestSuite}, Total Duration: {Duration}ms", 
                    GetType().Name, TestStopwatch?.ElapsedMilliseconds ?? 0);

                // Clean up old screenshots for the current fixture
                ScreenshotService?.CleanupOldScreenshots();
            }
            catch (Exception ex)
            {
                Logger?.Debug(ex, "Test suite teardown failed: {TestSuite}", GetType().Name);
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            try
            {
                var testName = TestContext.CurrentContext.Test.Name;
                var testDescription = TestContext.CurrentContext.Test.Properties.Get("Description")?.ToString() ?? TestDescription;

                // Set up Allure test context
                AllureContext.SetTestInfo(testName, testDescription, TestSeverity, TestCategory);
                TestStopwatch.Restart();

                // Set WebDriver for screenshot service if available
                if (WebDriver != null)
                {
                    ScreenshotService?.SetWebDriver(WebDriver);
                }

                Logger.Debug("Test setup completed: {TestName}", testName);
            }
            catch (Exception ex)
            {
                Logger?.Debug(ex, "Test setup failed: {TestName}", TestContext.CurrentContext.Test.Name);
                throw;
            }
        }

        [TearDown]
        public virtual void TearDown()
        {
            try
            {
                TestStopwatch?.Stop();
                var testName = TestContext.CurrentContext.Test.Name;
                var testResult = TestContext.CurrentContext.Result.Outcome.Status.ToString();

                // Take automatic screenshot based on configuration
                var screenshotPath = ScreenshotService?.TakeScreenshotIfNeeded(testName, testResult);
                
                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    Logger.Debug("Automatic screenshot taken: {ScreenshotPath}, Test: {TestName}, Result: {Result}", 
                        screenshotPath, testName, testResult);
                }

                Logger.Debug("Test teardown completed: {TestName}, Result: {Result}, Duration: {Duration}ms", 
                    testName, testResult, TestStopwatch?.ElapsedMilliseconds ?? 0);

                // Clear Allure context for the next test
                AllureContext.Clear();
            }
            catch (Exception ex)
            {
                Logger?.Debug(ex, "Test teardown failed: {TestName}", TestContext.CurrentContext.Test.Name);
            }
        }
        
        /// <summary>
        /// Logs test data for Allure reporting.
        /// </summary>
        /// <param name="testData">The test data to log.</param>
        /// <param name="name">The attachment name.</param>
        protected void LogTestData(object testData, string name = "Test Data")
        {
            try
            {
                AllureAttachmentHelper.AddTestDataAttachment(testData, name);
                Logger.Debug("Test data logged: {Name}", name);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to log test data: {Name}", name);
            }
        }

        /// <summary>
        /// Logs performance metrics for Allure reporting.
        /// </summary>
        /// <param name="metrics">The performance metrics to log.</param>
        /// <param name="name">The attachment name.</param>
        protected void LogPerformanceMetrics(object metrics, string name = "Performance Metrics")
        {
            try
            {
                AllureAttachmentHelper.AddPerformanceMetricsAttachment(metrics, name);
                Logger.Debug("Performance metrics logged: {Name}", name);
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to log performance metrics: {Name}", name);
            }
        }

        /// <summary>
        /// Takes a screenshot and attaches it to the Allure report.
        /// </summary>
        /// <param name="name">The name of the screenshot.</param>
        /// <param name="description">An optional description for the screenshot.</param>
        /// <returns>The path to the saved screenshot file, or null if failed.</returns>
        protected string TakeScreenshot(string name, string description = null)
        {
            if (WebDriver == null)
            {
                Logger.Debug("TakeScreenshot called, but WebDriver is not available.");
                return null;
            }

            try
            {
                var screenshotPath = ScreenshotService?.TakeManualScreenshot(name, description);
                if (!string.IsNullOrEmpty(screenshotPath))
                {
                    AllureAttachmentHelper.AddScreenshotFromFile(screenshotPath, name, description);
                    Logger.Debug("Manual screenshot taken and attached to report: {Name}", name);
                }
                return screenshotPath;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Failed to take manual screenshot: {Name}", name);
                return null;
            }
        }
    }
} 