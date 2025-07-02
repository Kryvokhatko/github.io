using CSTestFramework.Core.Configuration.Interfaces;

namespace CSTestFramework.Core.Configuration.Models
{
  

    /// <summary>
    /// Represents API test configuration settings.
    /// </summary>
    public class ApiSettings : IApiConfiguration
    {
        public string ApiUrl { get; set; }
        public int Timeout { get; set; }
        public int RetryCount { get; set; }
        public int RetryDelay { get; set; }
        public string ApiVersion { get; set; }
        public string AuthToken { get; set; }
        public bool EnableRequestLogging { get; set; }
        public int MaxConcurrentRequests { get; set; }

        public ApiSettings()
        {
            // Set default values if needed
            ApiUrl = "https://api.example.com";
            Timeout = 30; // seconds
            RetryCount = 3;
            RetryDelay = 500; // milliseconds
            ApiVersion = "v1";
            AuthToken = ""; // Should be overridden by environment variables
            EnableRequestLogging = false;
            MaxConcurrentRequests = 5;
        }
    }
} 