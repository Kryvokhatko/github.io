using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Logging;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Provides helper methods for adding attachments to Allure reports.
    /// </summary>
    public static class AllureAttachmentHelper
    {
        private static readonly ILogger _logger = TestLoggerHelper.CreateTestLogger("AllureAttachmentHelper");

        /// <summary>
        /// Adds a screenshot to the current test.
        /// </summary>
        /// <param name="screenshotBytes">The screenshot as byte array.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddScreenshot(byte[] screenshotBytes, string name = "Screenshot", string description = null)
        {
            try
            {
                AllureTestContext.Current.AddAttachment(name, screenshotBytes, "image/png", "png");
                _logger.Debug("Screenshot added to Allure report: {Name}, Size: {Size} bytes", 
                    name, screenshotBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add screenshot to Allure report: {Name}", name);
            }
        }

        /// <summary>
        /// Adds a screenshot from a file path to the current test.
        /// </summary>
        /// <param name="filePath">The path to the screenshot file.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddScreenshotFromFile(string filePath, string name = "Screenshot", string description = null)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.Debug("Screenshot file not found: {FilePath}", filePath);
                    return;
                }

                var imageBytes = File.ReadAllBytes(filePath);
                var extension = Path.GetExtension(filePath).TrimStart('.');
                var mimeType = GetMimeType(extension);

                AllureTestContext.Current.AddAttachment(name, imageBytes, mimeType, extension);
                _logger.Debug("Screenshot from file added to Allure report: {Name}, File: {FilePath}, Size: {Size} bytes", 
                    name, filePath, imageBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add screenshot from file to Allure report: {FilePath}", filePath);
            }
        }

        /// <summary>
        /// Adds JSON data as an attachment to the current test.
        /// </summary>
        /// <param name="data">The data object to serialize.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddJsonAttachment(object data, string name = "Data", string description = null)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                var jsonBytes = Encoding.UTF8.GetBytes(json);

                AllureTestContext.Current.AddAttachment(name, jsonBytes, "application/json", "json");
                _logger.Debug("JSON data added to Allure report: {Name}, Size: {Size} bytes", 
                    name, jsonBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add JSON data to Allure report: {Name}", name);
            }
        }

        /// <summary>
        /// Adds HTML content as an attachment to the current test.
        /// </summary>
        /// <param name="htmlContent">The HTML content.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddHtmlAttachment(string htmlContent, string name = "Page", string description = null)
        {
            try
            {
                var htmlBytes = Encoding.UTF8.GetBytes(htmlContent);
                AllureTestContext.Current.AddAttachment(name, htmlBytes, "text/html", "html");
                _logger.Debug("HTML content added to Allure report: {Name}, Size: {Size} bytes", 
                    name, htmlBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add HTML content to Allure report: {Name}", name);
            }
        }

        /// <summary>
        /// Adds log content as an attachment to the current test.
        /// </summary>
        /// <param name="logContent">The log content.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddLogAttachment(string logContent, string name = "Test Log", string description = null)
        {
            try
            {
                AllureTestContext.Current.AddTextAttachment(name, logContent, "log");
                _logger.Debug("Log content added to Allure report: {Name}, Size: {Size} characters", 
                    name, logContent?.Length ?? 0);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add log content to Allure report: {Name}", name);
            }
        }

        /// <summary>
        /// Adds a text file as an attachment to the current test.
        /// </summary>
        /// <param name="filePath">The path to the text file.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddTextFileAttachment(string filePath, string name = "File", string description = null)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    _logger.Debug("Text file not found: {FilePath}", filePath);
                    return;
                }

                var content = File.ReadAllText(filePath);
                var extension = Path.GetExtension(filePath).TrimStart('.');
                if (string.IsNullOrEmpty(extension))
                    extension = "txt";

                AllureTestContext.Current.AddTextAttachment(name, content, extension);
                _logger.Debug("Text file added to Allure report: {Name}, File: {FilePath}, Size: {Size} characters", 
                    name, filePath, content.Length);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add text file to Allure report: {FilePath}", filePath);
            }
        }

        /// <summary>
        /// Adds API request/response data as an attachment to the current test.
        /// </summary>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="url">The request URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <param name="responseBody">The response body.</param>
        /// <param name="statusCode">The response status code.</param>
        /// <param name="headers">Optional request/response headers.</param>
        public static void AddApiRequestResponseAttachment(string httpMethod, string url, string requestBody, 
            string responseBody, int statusCode, object headers = null)
        {
            try
            {
                var apiData = new
                {
                    Request = new
                    {
                        Method = httpMethod,
                        Url = url,
                        Headers = headers,
                        Body = requestBody
                    },
                    Response = new
                    {
                        StatusCode = statusCode,
                        Headers = headers,
                        Body = responseBody
                    },
                    Timestamp = DateTime.UtcNow
                };

                AddJsonAttachment(apiData, $"API Request-Response ({httpMethod} {url})", 
                    $"API interaction details for {httpMethod} {url}");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add API request/response attachment for {Method} {Url}", 
                    httpMethod, url);
            }
        }

        /// <summary>
        /// Adds test data as an attachment to the current test.
        /// </summary>
        /// <param name="testData">The test data object.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddTestDataAttachment(object testData, string name = "Test Data", string description = null)
        {
            try
            {
                AddJsonAttachment(testData, name, description ?? "Test execution data");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add test data attachment: {Name}", name);
            }
        }

        /// <summary>
        /// Adds performance metrics as an attachment to the current test.
        /// </summary>
        /// <param name="metrics">The performance metrics object.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="description">Optional description.</param>
        public static void AddPerformanceMetricsAttachment(object metrics, string name = "Performance Metrics", string description = null)
        {
            try
            {
                AddJsonAttachment(metrics, name, description ?? "Performance measurement data");
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to add performance metrics attachment: {Name}", name);
            }
        }

        /// <summary>
        /// Gets the MIME type for a file extension.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The MIME type.</returns>
        private static string GetMimeType(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case "png":
                    return "image/png";
                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                case "gif":
                    return "image/gif";
                case "bmp":
                    return "image/bmp";
                case "svg":
                    return "image/svg+xml";
                case "html":
                case "htm":
                    return "text/html";
                case "css":
                    return "text/css";
                case "js":
                    return "application/javascript";
                case "json":
                    return "application/json";
                case "xml":
                    return "application/xml";
                case "txt":
                    return "text/plain";
                case "csv":
                    return "text/csv";
                case "pdf":
                    return "application/pdf";
                default:
                    return "application/octet-stream";
            }
        }
    }
} 