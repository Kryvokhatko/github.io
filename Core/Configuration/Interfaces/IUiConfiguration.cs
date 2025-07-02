namespace CSTestFramework.Core.Configuration.Interfaces
{
    /// <summary>
    /// Interface for UI test configuration settings.
    /// </summary>
    public interface IUiConfiguration
    {
        /// <summary>
        /// Gets the browser type to use for UI tests (e.g., "Chrome", "Firefox", "Edge").
        /// </summary>
        string Browser { get; }

        /// <summary>
        /// Gets a value indicating whether to run the browser in headless mode.
        /// </summary>
        bool Headless { get; }

        /// <summary>
        /// Gets the implicit wait time in seconds for UI elements.
        /// </summary>
        int ImplicitWait { get; }

        /// <summary>
        /// Gets the page load timeout in seconds.
        /// </summary>
        int PageLoadTimeout { get; }

        /// <summary>
        /// Gets the script timeout in seconds.
        /// </summary>
        int ScriptTimeout { get; }

        /// <summary>
        /// Gets the browser window width.
        /// </summary>
        int WindowWidth { get; }

        /// <summary>
        /// Gets the browser window height.
        /// </summary>
        int WindowHeight { get; }

        /// <summary>
        /// Gets a value indicating whether to take screenshots on test failure.
        /// </summary>
        bool TakeScreenshotsOnFailure { get; }

        /// <summary>
        /// Gets the path where screenshots will be saved.
        /// </summary>
        string ScreenshotPath { get; }

        /// <summary>
        /// Gets the browser download directory path.
        /// </summary>
        string DownloadPath { get; }
    }
} 