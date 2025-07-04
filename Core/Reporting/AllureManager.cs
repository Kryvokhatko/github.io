using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Configuration.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Allure.Net.Commons;
using CSTestFramework.Core.Configuration.Models;
using CSTestFramework.Core.Configuration;

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
                
                // Ensure directory exists and is empty with retry logic
                SafeDeleteDirectory(_resultsDirectory);
                SafeCreateDirectory(_resultsDirectory);

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
        /// Safely deletes a directory with retry logic to handle file access issues.
        /// </summary>
        private static void SafeDeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            const int maxRetries = 5;
            const int delayMs = 500;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // Try to delete any files in the directory first
                    foreach (var file in Directory.GetFiles(path))
                    {
                        File.SetAttributes(file, FileAttributes.Normal);
                        SafeDeleteFile(file);
                    }

                    // Then delete subdirectories
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        SafeDeleteDirectory(dir);
                    }

                    // Finally delete the directory itself
                    Directory.Delete(path, false);
                    return;
                }
                catch (IOException)
                {
                    if (i < maxRetries - 1)
                    {
                        _logger?.Debug("Failed to delete directory {Path}, retrying in {Delay}ms...", path, delayMs);
                        Thread.Sleep(delayMs);
                    }
                    else
                    {
                        _logger?.Debug("Failed to delete directory {Path} after {MaxRetries} attempts", path, maxRetries);
                        // Continue without throwing - we'll try to work with existing directory
                    }
                }
            }
        }

        /// <summary>
        /// Safely deletes a file with retry logic to handle file access issues.
        /// </summary>
        private static void SafeDeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            const int maxRetries = 5;
            const int delayMs = 500;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    File.Delete(path);
                    return;
                }
                catch (IOException)
                {
                    if (i < maxRetries - 1)
                    {
                        Thread.Sleep(delayMs);
                    }
                }
            }
        }

        /// <summary>
        /// Safely creates a directory with retry logic to handle file access issues.
        /// </summary>
        private static void SafeCreateDirectory(string path)
        {
            const int maxRetries = 5;
            const int delayMs = 500;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    return;
                }
                catch (IOException)
                {
                    if (i < maxRetries - 1)
                    {
                        _logger?.Debug("Failed to create directory {Path}, retrying in {Delay}ms...", path, delayMs);
                        Thread.Sleep(delayMs);
                    }
                    else
                    {
                        _logger?.Debug("Failed to create directory {Path} after {MaxRetries} attempts", path, maxRetries);
                        throw; // Finally throw if we can't create the directory
                    }
                }
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

                SafeWriteAllText(envFilePath, envContent);
                _logger.Debug("Environment info written to: {FilePath}", envFilePath);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to write environment info");
            }
        }

        /// <summary>
        /// Safely writes text to a file with retry logic to handle file access issues.
        /// </summary>
        private static void SafeWriteAllText(string path, string content)
        {
            const int maxRetries = 5;
            const int delayMs = 500;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    File.WriteAllText(path, content);
                    return;
                }
                catch (IOException)
                {
                    if (i < maxRetries - 1)
                    {
                        _logger?.Debug("Failed to write to file {Path}, retrying in {Delay}ms...", path, delayMs);
                        Thread.Sleep(delayMs);
                    }
                    else
                    {
                        _logger?.Debug("Failed to write to file {Path} after {MaxRetries} attempts", path, maxRetries);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Generates the Allure report from the results directory.
        /// </summary>
        public static void GenerateReport()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException("AllureManager must be initialized before generating report.");
            }

            try
            {
                _logger.Debug("Generating Allure report from results in: {Directory}", _resultsDirectory);
                
                // The actual report generation would typically be handled by the Allure CLI
                // This method is primarily a placeholder for any pre/post processing
                
                _logger.Debug("Allure report generation complete");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to generate Allure report");
                throw;
            }
        }

        /// <summary>
        /// Cleans up resources used by the Allure manager.
        /// </summary>
        public static void Dispose()
        {
            _isInitialized = false;
            _logger = null;
            _loggingConfig = null;
            _resultsDirectory = null;
        }

        /// <summary>
        /// Builds environment information for the Allure report based on the current test environment.
        /// </summary>
        /// <param name="driver">The WebDriver instance, if available.</param>
        /// <returns>The environment information.</returns>
        public static AllureEnvironmentInfo BuildEnvironmentInfo(IWebDriver driver = null)
        {
            var info = new AllureEnvironmentInfo
            {
                OperatingSystem = Environment.OSVersion.ToString(),
                Framework = "CSTestFramework",
                Language = "C#",
                BaseUrl = ConfigurationManager.Instance.AppSettings.Ui.ApplicationUrl
            };

            if (driver != null)
            {
                info.Browser = driver.GetType().Name.Replace("Driver", "");
                
                if (driver is RemoteWebDriver remoteDriver)
                {
                    info.BrowserVersion = remoteDriver.Capabilities.GetCapability("browserVersion") as string
                        ?? remoteDriver.Capabilities.GetCapability("version") as string;
                }
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