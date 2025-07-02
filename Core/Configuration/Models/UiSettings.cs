using CSTestFramework.Core.Configuration.Interfaces;

namespace CSTestFramework.Core.Configuration.Models
{
    /// <summary>
    /// Represents UI test configuration settings.
    /// </summary>
    public class UiSettings : IUiConfiguration
    {
        public string ApplicationUrl { get; set; } // Renamed from BaseUrl for clarity
        public string Browser { get; set; }
        public bool Headless { get; set; }
        public int ImplicitWait { get; set; }
        public int PageLoadTimeout { get; set; }
        public int ScriptTimeout { get; set; }
        public int WindowWidth { get; set; } // default: 0 (means "not set")
        public int WindowHeight { get; set; }  // default: 0 (means "not set")
        public bool TakeScreenshotsOnFailure { get; set; }
        public string ScreenshotPath { get; set; }
        public string DownloadPath { get; set; }

        public UiSettings()
        {
            // Set default values if needed
            ApplicationUrl = "http://localhost:3000"; // Default URL for the web application
            Browser = "Chrome";
            Headless = false;
            ImplicitWait = 10; // seconds
            PageLoadTimeout = 30; // seconds
            ScriptTimeout = 30; // seconds
            WindowWidth = 0; // 0 means "not set", so maximize by default
            WindowHeight = 0; // 0 means "not set", so maximize by default
            TakeScreenshotsOnFailure = true;
            ScreenshotPath = "allure-results/screenshots";
            DownloadPath = "downloads";
        }
    }
} 