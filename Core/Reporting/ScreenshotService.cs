using System;
using System.IO;
using OpenQA.Selenium;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Configuration.Models;
using CSTestFramework.Core.Logging;
using System.Threading;
using NUnit.Framework;
using Newtonsoft.Json;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Service for handling automatic screenshot capture during test execution using Selenium WebDriver.
    /// </summary>
    public class ScreenshotService
    {
        private readonly ILogger _logger;
        private readonly LoggingSettings _settings;
        private IWebDriver _webDriver;

        /// <summary>
        /// Initializes a new instance of the ScreenshotService.
        /// </summary>
        /// <param name="settings">The logging settings containing screenshot configuration.</param>
        public ScreenshotService(LoggingSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _logger = TestLoggerHelper.CreateTestLogger("ScreenshotService");
            
            // Ensure screenshot directory exists
            if (!string.IsNullOrEmpty(_settings.ScreenshotDirectory))
            {
                Directory.CreateDirectory(_settings.ScreenshotDirectory);
            }
        }

        /// <summary>
        /// Sets the WebDriver instance for taking screenshots.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance.</param>
        public void SetWebDriver(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        /// <summary>
        /// Takes a screenshot based on the current configuration and test result.
        /// </summary>
        /// <param name="testName">The name of the test.</param>
        /// <param name="testResult">The test result (Passed, Failed, etc.).</param>
        /// <returns>The screenshot file path if taken, null otherwise.</returns>
        public string TakeScreenshotIfNeeded(string testName, string testResult)
        {
            try
            {
                // Check if screenshot should be taken based on configuration
                if (!ShouldTakeScreenshot(testResult))
                {
                    _logger.Debug("Screenshot not needed for test: {TestName}, Result: {Result}, Mode: {Mode}", 
                        testName, testResult, _settings.ScreenshotMode);
                    return null;
                }

                string screenshotPath;

                // Check if WebDriver is available for browser screenshots
                if (_webDriver != null)
                {
                    // UI Test: Take actual browser screenshot
                    screenshotPath = TakeBrowserScreenshot(testName, testResult);
                }
                else
                {
                    // API Test: Take context screenshot (test data, logs, etc.)
                    screenshotPath = TakeContextScreenshot(testName, testResult);
                }
                
                // Add to Allure if configured
                if (_settings.IncludeScreenshotInAllure && !string.IsNullOrEmpty(screenshotPath))
                {
                    AddScreenshotToAllure(screenshotPath, testName, testResult);
                }

                return screenshotPath;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to take screenshot for test: {TestName}", testName);
                return null;
            }
        }

        /// <summary>
        /// Determines if a screenshot should be taken based on configuration and test result.
        /// </summary>
        /// <param name="testResult">The test result.</param>
        /// <returns>True if screenshot should be taken, false otherwise.</returns>
        private bool ShouldTakeScreenshot(string testResult)
        {
            switch (_settings.ScreenshotMode)
            {
                case ScreenshotMode.Never:
                    return false;
                
                case ScreenshotMode.OnFailureOnly:
                    return testResult.Equals("Failed", StringComparison.OrdinalIgnoreCase);
                
                case ScreenshotMode.Always:
                    return true;
                
                default:
                    return false;
            }
        }

        /// <summary>
        /// Takes a browser screenshot using Selenium WebDriver.
        /// </summary>
        /// <param name="testName">The test name.</param>
        /// <param name="testResult">The test result.</param>
        /// <returns>The path to the saved screenshot file.</returns>
        private string TakeBrowserScreenshot(string testName, string testResult)
        {
            try
            {
                // Take screenshot using Selenium WebDriver
                var screenshot = ((ITakesScreenshot)_webDriver).GetScreenshot();
                var screenshotBytes = screenshot.AsByteArray;

                // Check file size limit
                if (screenshotBytes.Length > _settings.MaxScreenshotSize)
                {
                    _logger.Debug("Screenshot size ({Size} bytes) exceeds limit ({Limit} bytes) for test: {TestName}", 
                        screenshotBytes.Length, _settings.MaxScreenshotSize, testName);
                }

                // Generate filename
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var safeTestName = MakeSafeFileName(testName);
                var filename = $"{safeTestName}_{testResult}_{timestamp}.{_settings.ScreenshotFormat}";
                var filePath = Path.Combine(_settings.ScreenshotDirectory, filename);

                // Save screenshot
                File.WriteAllBytes(filePath, screenshotBytes);

                _logger.Debug("Browser screenshot saved: {FilePath}, Size: {Size} bytes, Test: {TestName}", 
                    filePath, screenshotBytes.Length, testName);

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to take browser screenshot for test: {TestName}", testName);
                return null;
            }
        }

        /// <summary>
        /// Takes a context screenshot for API tests (test data, logs, etc.).
        /// </summary>
        /// <param name="testName">The test name.</param>
        /// <param name="testResult">The test result.</param>
        /// <returns>The path to the saved context file.</returns>
        private string TakeContextScreenshot(string testName, string testResult)
        {
            try
            {
                // Create context data for API tests
                var contextData = new
                {
                    TestName = testName,
                    TestResult = testResult,
                    Timestamp = DateTime.UtcNow,
                    Environment = Environment.MachineName,
                    ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
                    ThreadId = Thread.CurrentThread.ManagedThreadId,
                    MemoryUsage = GC.GetTotalMemory(false),
                    TestContext = new
                    {
                        // Add any available test context information
                        TestMethod = TestContext.CurrentContext?.Test?.MethodName,
                        TestClass = TestContext.CurrentContext?.Test?.ClassName,
                        TestDescription = TestContext.CurrentContext?.Test?.Properties?.Get("Description")?.ToString(),
                        TestCategories = TestContext.CurrentContext?.Test?.Properties?.Get("Category")?.ToString()
                    }
                };

                // Serialize context data to JSON
                var contextJson = JsonConvert.SerializeObject(contextData, Formatting.Indented);
                var contextBytes = System.Text.Encoding.UTF8.GetBytes(contextJson);

                // Check file size limit
                if (contextBytes.Length > _settings.MaxScreenshotSize)
                {
                    _logger.Debug("Context data size ({Size} bytes) exceeds limit ({Limit} bytes) for test: {TestName}", 
                        contextBytes.Length, _settings.MaxScreenshotSize, testName);
                }

                // Generate filename for context data
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var safeTestName = MakeSafeFileName(testName);
                var filename = $"{safeTestName}_{testResult}_Context_{timestamp}.json";
                var filePath = Path.Combine(_settings.ScreenshotDirectory, filename);

                // Save context data
                File.WriteAllBytes(filePath, contextBytes);

                _logger.Debug("Context screenshot saved: {FilePath}, Size: {Size} bytes, Test: {TestName}", 
                    filePath, contextBytes.Length, testName);

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to take context screenshot for test: {TestName}", testName);
                return null;
            }
        }

        /// <summary>
        /// Adds a screenshot to the Allure report.
        /// </summary>
        /// <param name="screenshotPath">The path to the screenshot file.</param>
        /// <param name="testName">The test name.</param>
        /// <param name="testResult">The test result.</param>
        private void AddScreenshotToAllure(string screenshotPath, string testName, string testResult)
        {
            try
            {
                if (!File.Exists(screenshotPath))
                {
                    _logger.Debug("Screenshot file not found for Allure: {FilePath}", screenshotPath);
                    return;
                }

                var screenshotBytes = File.ReadAllBytes(screenshotPath);
                var fileExtension = Path.GetExtension(screenshotPath).TrimStart('.');
                var isContextScreenshot = fileExtension.Equals("json", StringComparison.OrdinalIgnoreCase);

                string attachmentName;
                string description;
                string mimeType;

                if (isContextScreenshot)
                {
                    // Context screenshot for API tests
                    attachmentName = $"Context_{testResult}_{testName}";
                    description = $"Test context data for {testName} (Result: {testResult})";
                    mimeType = "application/json";
                }
                else
                {
                    // Browser screenshot for UI tests
                    attachmentName = $"Screenshot_{testResult}_{testName}";
                    description = $"Browser screenshot for {testName} (Result: {testResult})";
                    mimeType = "image/png";
                }

                AllureTestContext.Current.AddAttachment(attachmentName, screenshotBytes, mimeType, fileExtension);
                
                _logger.Debug("Screenshot added to Allure report: {Name}, Test: {TestName}", attachmentName, testName);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add screenshot to Allure report: {FilePath}", screenshotPath);
            }
        }

        /// <summary>
        /// Makes a filename safe by removing invalid characters.
        /// </summary>
        /// <param name="filename">The original filename.</param>
        /// <returns>A safe filename.</returns>
        private string MakeSafeFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return "test";

            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = filename;

            foreach (var invalidChar in invalidChars)
            {
                safeName = safeName.Replace(invalidChar, '_');
            }

            // Limit length to avoid path too long issues
            if (safeName.Length > 100)
            {
                safeName = safeName.Substring(0, 100);
            }

            return safeName;
        }

        /// <summary>
        /// Takes a manual screenshot during test execution.
        /// </summary>
        /// <param name="name">The name for the screenshot.</param>
        /// <param name="description">Optional description.</param>
        /// <returns>The path to the saved screenshot file.</returns>
        public string TakeManualScreenshot(string name, string description = null)
        {
            try
            {
                string filePath;
                byte[] fileBytes;
                string fileExtension;

                if (_webDriver != null)
                {
                    // UI Test: Take actual browser screenshot
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var safeName = MakeSafeFileName(name);
                    var filename = $"{safeName}_{timestamp}.{_settings.ScreenshotFormat}";
                    filePath = Path.Combine(_settings.ScreenshotDirectory, filename);

                    var screenshot = ((ITakesScreenshot)_webDriver).GetScreenshot();
                    fileBytes = screenshot.AsByteArray;
                    fileExtension = _settings.ScreenshotFormat;

                    _logger.Debug("Manual browser screenshot saved: {FilePath}, Name: {Name}", filePath, name);
                }
                else
                {
                    // API Test: Take context screenshot
                    var contextData = new
                    {
                        ManualScreenshotName = name,
                        Description = description,
                        Timestamp = DateTime.UtcNow,
                        Environment = Environment.MachineName,
                        ProcessId = System.Diagnostics.Process.GetCurrentProcess().Id,
                        ThreadId = Thread.CurrentThread.ManagedThreadId,
                        MemoryUsage = GC.GetTotalMemory(false),
                        TestContext = new
                        {
                            TestMethod = TestContext.CurrentContext?.Test?.MethodName,
                            TestClass = TestContext.CurrentContext?.Test?.ClassName,
                            TestDescription = TestContext.CurrentContext?.Test?.Properties?.Get("Description")?.ToString(),
                            TestCategories = TestContext.CurrentContext?.Test?.Properties?.Get("Category")?.ToString()
                        }
                    };

                    var contextJson = JsonConvert.SerializeObject(contextData, Formatting.Indented);
                    fileBytes = System.Text.Encoding.UTF8.GetBytes(contextJson);
                    fileExtension = "json";

                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var safeName = MakeSafeFileName(name);
                    var filename = $"{safeName}_Manual_{timestamp}.{fileExtension}";
                    filePath = Path.Combine(_settings.ScreenshotDirectory, filename);

                    _logger.Debug("Manual context screenshot saved: {FilePath}, Name: {Name}", filePath, name);
                }

                // Save file
                File.WriteAllBytes(filePath, fileBytes);

                // Add to Allure if configured
                if (_settings.IncludeScreenshotInAllure)
                {
                    var mimeType = fileExtension.Equals("json", StringComparison.OrdinalIgnoreCase) 
                        ? "application/json" 
                        : "image/png";
                    
                    AllureTestContext.Current.AddAttachment(name, fileBytes, mimeType, fileExtension);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to take manual screenshot: {Name}", name);
                return null;
            }
        }

        /// <summary>
        /// Cleans up old screenshots based on configuration.
        /// </summary>
        /// <param name="maxAgeInDays">Maximum age of screenshots to keep.</param>
        public void CleanupOldScreenshots(int maxAgeInDays = 30)
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.ScreenshotDirectory) || !Directory.Exists(_settings.ScreenshotDirectory))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-maxAgeInDays);
                var files = Directory.GetFiles(_settings.ScreenshotDirectory, $"*.{_settings.ScreenshotFormat}");

                var deletedCount = 0;
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                        deletedCount++;
                    }
                }

                if (deletedCount > 0)
                {
                    _logger.Debug("Cleaned up {Count} old screenshots older than {Days} days", deletedCount, maxAgeInDays);
                }
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to cleanup old screenshots");
            }
        }
    }
} 