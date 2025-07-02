using System;
using System.IO;
using System.Reflection;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Configuration.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

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
                // Set Allure results directory
                var resultsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "allure-results");
                Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIR", resultsDirectory);

                // Ensure results directory exists
                Directory.CreateDirectory(resultsDirectory);

                // Set Allure configuration file path
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "allureConfig.json");
                if (File.Exists(configPath))
                {
                    Environment.SetEnvironmentVariable("ALLURE_CONFIG", configPath);
                }

                _logger.Debug("Allure reporting initialized. Results directory: {ResultsDir}", resultsDirectory);
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
        public static void AddEnvironmentInfo(EnvironmentInfo environmentInfo)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("AllureManager must be initialized before adding environment info.");
            }

            try
            {
                var resultsDir = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIR");
                var environmentFile = Path.Combine(resultsDir, "environment.properties");

                var environmentData = new[]
                {
                    $"Browser={environmentInfo.Browser}",
                    $"BrowserVersion={environmentInfo.BrowserVersion}",
                    $"OS={environmentInfo.OperatingSystem}",
                    $"OSVersion={environmentInfo.OSVersion}",
                    $"Framework={environmentInfo.Framework}",
                    $"FrameworkVersion={environmentInfo.FrameworkVersion}",
                    $"TestEnvironment={environmentInfo.TestEnvironment}",
                    $"BaseUrl={environmentInfo.BaseUrl}"
                };

                File.WriteAllLines(environmentFile, environmentData);
                _logger.Debug("Environment information added to Allure report");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add environment information to Allure report");
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
                var resultsDir = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIR");
                if (string.IsNullOrEmpty(outputDirectory))
                {
                    outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "allure-report");
                }

                // Note: In a real implementation, you would call the Allure command-line tool
                // For now, we'll just log that the report generation is requested
                _logger.Debug("Allure report generation requested. Results: {ResultsDir}, Output: {OutputDir}", 
                    resultsDir, outputDirectory);

                // In CI/CD, you would typically run: allure generate allure-results --clean -o allure-report
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

        public static EnvironmentInfo BuildEnvironmentInfo(IWebDriver driver = null)
        {
            string browser = "Unknown";
            string browserVersion = "Unknown";
            if (driver is RemoteWebDriver remoteDriver)
            {
                browser = remoteDriver.Capabilities.GetCapability("browserName")?.ToString() ?? "Unknown";
                browserVersion = remoteDriver.Capabilities.GetCapability("browserVersion")?.ToString()
                    ?? remoteDriver.Capabilities.GetCapability("version")?.ToString()
                    ?? "Unknown";
            }
            var config = CSTestFramework.Core.Configuration.ConfigurationManager.Instance.AppSettings;
            return new EnvironmentInfo
            {
                Browser = browser,
                BrowserVersion = browserVersion,
                OperatingSystem = Environment.OSVersion.Platform.ToString(),
                OSVersion = Environment.OSVersion.VersionString,
                Framework = "NUnit",
                FrameworkVersion = typeof(NUnit.Framework.TestFixtureAttribute).Assembly.GetName().Version?.ToString() ?? "Unknown",
                TestEnvironment = config.Environment.EnvironmentName,
                BaseUrl = config.Ui?.ApplicationUrl ?? config.Api?.ApiUrl
            };
        }
    }

    /// <summary>
    /// Represents environment information for Allure reports.
    /// </summary>
    public class EnvironmentInfo
    {
        public string Browser { get; set; } = "Unknown";
        public string BrowserVersion { get; set; } = "Unknown";
        public string OperatingSystem { get; set; } = Environment.OSVersion.Platform.ToString();
        public string OSVersion { get; set; } = Environment.OSVersion.VersionString;
        public string Framework { get; set; } = "NUnit";
        public string FrameworkVersion { get; set; } = typeof(TestFixtureAttribute).Assembly.GetName().Version?.ToString() ?? "Unknown";
        public string TestEnvironment { get; set; } = "Development";
        public string BaseUrl { get; set; } = "http://localhost";
    }
} 