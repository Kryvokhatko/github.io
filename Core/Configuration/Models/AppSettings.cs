using CSTestFramework.Core.Configuration.Models;

namespace CSTestFramework.Core.Configuration.Models
{
    /// <summary>
    /// Represents the root application settings, containing various configuration sections.
    /// </summary>
    public class AppSettings
    {
        public EnvironmentSettings Environment { get; set; }
        public ApiSettings Api { get; set; }
        public UiSettings Ui { get; set; }
        public LoggingSettings Logging { get; set; }

        public AppSettings()
        {
            // Initialize child settings to avoid NullReferenceExceptions if not configured
            Environment = new EnvironmentSettings();
            Api = new ApiSettings();
            Ui = new UiSettings();
            Logging = new LoggingSettings();
        }
    }
} 