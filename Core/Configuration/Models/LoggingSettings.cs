using CSTestFramework.Core.Logging.Interfaces;

namespace CSTestFramework.Core.Configuration.Models
{
    /// <summary>
    /// Represents logging configuration settings.
    /// </summary>
    public class LoggingSettings : ILoggingConfiguration
    {
        public string MinimumLevel { get; set; }
        public bool EnableConsoleLogging { get; set; }
        public bool EnableFileLogging { get; set; }
        public string LogFilePath { get; set; }
        public string LogFileTemplate { get; set; }
        public string LogFormat { get; set; }
        public long MaxFileSize { get; set; }
        public int MaxFileCount { get; set; }
        public bool IncludeStructuredProperties { get; set; }
        
        // Screenshot configuration
        public ScreenshotMode ScreenshotMode { get; set; }
        public string ScreenshotDirectory { get; set; }
        public string ScreenshotFormat { get; set; }
        public bool IncludeScreenshotInAllure { get; set; }
        public int MaxScreenshotSize { get; set; }

        public LoggingSettings()
        {
            // Set default values if needed
            EnableConsoleLogging = true;
            EnableFileLogging = true;
            LogFilePath = "logs";
            LogFileTemplate = "log-.txt";
            LogFormat = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
            MaxFileSize = 10L * 1024 * 1024; // 10MB
            MaxFileCount = 31; // Keep 31 days of logs
            IncludeStructuredProperties = true;
            MinimumLevel = "Debug";
            
            // Screenshot defaults
            ScreenshotMode = ScreenshotMode.OnFailureOnly;
            ScreenshotDirectory = "screenshots";
            ScreenshotFormat = "png";
            IncludeScreenshotInAllure = true;
            MaxScreenshotSize = 5 * 1024 * 1024; // 5MB
        }
    }

    /// <summary>
    /// Defines when screenshots should be taken during test execution.
    /// </summary>
    public enum ScreenshotMode
    {
        /// <summary>
        /// Never take screenshots automatically.
        /// </summary>
        Never,
        
        /// <summary>
        /// Take screenshots only when tests fail.
        /// </summary>
        OnFailureOnly,
        
        /// <summary>
        /// Take screenshots for all tests (pass and fail).
        /// </summary>
        Always
    }
} 