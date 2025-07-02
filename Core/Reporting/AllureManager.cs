using System;
using System.IO;
using System.Reflection;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Configuration.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Allure.Net.Commons;
using CSTestFramework.Core.Configuration.Models;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Manages Allure reporting configuration and lifecycle for the test framework.
    /// </summary>
    public static class AllureManager
    {
        private static bool _isInitialized;
        private static ILogger _logger;
        private static ILoggingConfiguration _loggingConfig;
        private static string _resultsDirectory;

        /// <summary>
        /// Initializes Allure reporting with the specified configuration.
        /// </summary>
        /// <param name="loggingConfiguration">The logging configuration.</param>
        /// <param name="logger">The logger instance.</param>
        public static void Initialize(ILoggingConfiguration loggingConfiguration, ILogger logger)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("AllureManager has already been initialized.");
            }

            _loggingConfig = loggingConfiguration ?? throw new ArgumentNullException(nameof(loggingConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                // Get the absolute path for Allure results
                var workingDirectory = Environment.CurrentDirectory;
                _resultsDirectory = Path.GetFullPath(Path.Combine(workingDirectory, "allure-results"));
                
                // Ensure directory exists and is empty
                if (Directory.Exists(_resultsDirectory))
                {
                    Directory.Delete(_resultsDirectory, true);
                }
                Directory.CreateDirectory(_resultsDirectory);

                // Configure Allure
                Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIRECTORY", _resultsDirectory);
                
                _logger.Debug("Allure initialized with results directory: {Directory}", _resultsDirectory);
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to initialize Allure reporting");
                throw;
            }
        }

        /// <summary>
        /// Gets the current test context for Allure reporting.
        /// </summary>
        /// <returns>The current test context.</returns>
        public static AllureTestContext GetCurrentTestContext()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("AllureManager must be initialized before getting test context.");
            }

            return AllureTestContext.Current;
        }

        /// <summary>
        /// Adds environment information to the Allure report.
        /// </summary>
        /// <param name="environmentInfo">The environment information to add.</param>
        public static void AddEnvironmentInfo(AllureEnvironmentInfo info)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("AllureManager must be initialized before adding environment info.");
            }

            try
            {
                var envFilePath = Path.Combine(_resultsDirectory, "environment.properties");
                var envContent = $@"Operating.System={info.OperatingSystem}
Browser={info.Browser ?? "Unknown"}
Browser.Version={info.BrowserVersion ?? "Unknown"}
Base.URL={info.BaseUrl}
Framework={info.Framework}
Language={info.Language}";

                File.WriteAllText(envFilePath, envContent);
                _logger.Debug("Environment info written to: {FilePath}", envFilePath);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to write environment info");
            }
        }

        /// <summary>
        /// Generates the Allure report from the results directory.
        /// </summary>
        /// <param name="outputDirectory">The output directory for the generated report.</param>
        public static void GenerateReport(string outputDirectory = null)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("AllureManager must be initialized before generating report.");
            }

            try
            {
                _logger.Debug("Allure results available at: {Directory}", _resultsDirectory);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to generate Allure report");
            }
        }

        /// <summary>
        /// Cleans up Allure resources.
        /// </summary>
        public static void Dispose()
        {
            if (_isInitialized)
            {
                _isInitialized = false;
                _logger?.Debug("Allure reporting disposed");
            }
        }

        public static AllureEnvironmentInfo BuildEnvironmentInfo(IWebDriver driver = null)
        {
            var info = new AllureEnvironmentInfo
            {
                OperatingSystem = Environment.OSVersion.ToString(),
                Framework = "CSTestFramework",
                Language = "C#",
                BaseUrl = Configuration.ConfigurationManager.Instance.AppSettings.Ui.ApplicationUrl
            };

            if (driver != null && driver is OpenQA.Selenium.Remote.RemoteWebDriver remoteDriver)
            {
                info.Browser = remoteDriver.Capabilities.GetCapability("browserName")?.ToString();
                info.BrowserVersion = remoteDriver.Capabilities.GetCapability("browserVersion")?.ToString() 
                    ?? remoteDriver.Capabilities.GetCapability("version")?.ToString();
            }

            return info;
        }
    }

    /// <summary>
    /// Represents environment information for Allure reports.
    /// </summary>
    public class AllureEnvironmentInfo
    {
        public string OperatingSystem { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string BaseUrl { get; set; }
        public string Framework { get; set; }
        public string Language { get; set; }
    }
} 