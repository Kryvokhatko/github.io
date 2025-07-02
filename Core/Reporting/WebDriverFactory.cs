using System;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using CSTestFramework.Core.Configuration.Models;
using CSTestFramework.Core.Logging;
using CSTestFramework.Core.Logging.Interfaces;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Factory class for creating WebDriver instances with proper configuration.
    /// </summary>
    public static class WebDriverFactory
    {
        private static readonly ILogger _logger = TestLoggerHelper.CreateTestLogger("WebDriverFactory");

        /// <summary>
        /// Creates a new WebDriver instance based on the specified browser type.
        /// </summary>
        /// <param name="browserType">The type of browser to create.</param>
        /// <param name="settings">Optional UI settings for configuring the browser.</param>
        /// <returns>A configured WebDriver instance.</returns>
        public static IWebDriver Create(BrowserType browserType, UiSettings settings = null)
        {
            _logger.Debug("Creating WebDriver for browser: {Browser}", browserType);
            try
            {
                // Use default settings if none provided
                if (settings == null)
                    settings = new UiSettings();

                // Create and configure the driver based on browser type
                IWebDriver driver = CreateDriver(browserType, settings);

                // Configure common settings
                ConfigureDriver(driver, settings);

                _logger.Debug("WebDriver created successfully for {Browser}", browserType);
                return driver;
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Failed to create WebDriver for browser: {Browser}", browserType);
                throw;
            }
        }

        /// <summary>
        /// Creates the appropriate WebDriver instance for the specified browser type.
        /// </summary>
        private static IWebDriver CreateDriver(BrowserType browserType, UiSettings settings)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return CreateChromeDriver(settings);
                case BrowserType.Firefox:
                    return CreateFirefoxDriver(settings);
                case BrowserType.Edge:
                    return CreateEdgeDriver(settings);
                default:
                    throw new ArgumentException($"Unsupported browser type: {browserType}");
            }
        }

        /// <summary>
        /// Creates a Chrome WebDriver instance with the specified settings.
        /// </summary>
        private static IWebDriver CreateChromeDriver(UiSettings settings)
        {
            new DriverManager().SetUpDriver(new ChromeConfig());
            var options = new ChromeOptions();

            if (settings.Headless)
            {
                options.AddArgument("--headless=new");
            }

            // Add common Chrome-specific arguments
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-infobars");

            //This code is for configuring the browser to automatically save files downloaded during automated
            //UI tests to a specific directory. Downloads happen automatically, so your tests don�t get stuck on browser prompts.
            // Configure download directory
            //if (!string.IsNullOrEmpty(settings.DownloadPath))
            //{
            //    var downloadPath = Path.GetFullPath(settings.DownloadPath);
            //    Directory.CreateDirectory(downloadPath);
            //    options.AddUserProfilePreference("download.default_directory", downloadPath);
            //    options.AddUserProfilePreference("download.prompt_for_download", false);
            //}

            return new ChromeDriver(options);
        }

        /// <summary>
        /// Creates a Firefox WebDriver instance with the specified settings.
        /// </summary>
        private static IWebDriver CreateFirefoxDriver(UiSettings settings)
        {
            new DriverManager().SetUpDriver(new FirefoxConfig());
            var options = new FirefoxOptions();

            if (settings.Headless)
            {
                options.AddArgument("--headless");
            }

            //This code is for configuring the browser to automatically save files downloaded during automated
            //UI tests to a specific directory. Downloads happen automatically, so your tests don�t get stuck on browser prompts.
            // Configure download directory
            //if (!string.IsNullOrEmpty(settings.DownloadPath))
            //{
            //    var downloadPath = Path.GetFullPath(settings.DownloadPath);
            //    Directory.CreateDirectory(downloadPath);
            //    options.SetPreference("browser.download.folderList", 2);
            //    options.SetPreference("browser.download.dir", downloadPath);
            //    options.SetPreference("browser.download.useDownloadDir", true);
            //    options.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream");
            //}

            return new FirefoxDriver(options);
        }

        /// <summary>
        /// Creates an Edge WebDriver instance with the specified settings.
        /// </summary>
        private static IWebDriver CreateEdgeDriver(UiSettings settings)
        {
            new DriverManager().SetUpDriver(new EdgeConfig());
            var options = new EdgeOptions();

            if (settings.Headless)
            {
                options.AddArgument("--headless=new");
            }

            // Add common Edge-specific arguments
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-infobars");

            //This code is for configuring the browser to automatically save files downloaded during automated
            //UI tests to a specific directory. Downloads happen automatically, so your tests don�t get stuck on browser prompts.
            // Configure download directory
            //if (!string.IsNullOrEmpty(settings.DownloadPath))
            //{
            //    var downloadPath = Path.GetFullPath(settings.DownloadPath);
            //    Directory.CreateDirectory(downloadPath);
            //    options.AddUserProfilePreference("download.default_directory", downloadPath);
            //    options.AddUserProfilePreference("download.prompt_for_download", false);
            //}

            return new EdgeDriver(options);
        }

        /// <summary>
        /// Configures common settings for the WebDriver instance.
        /// </summary>
        private static void ConfigureDriver(IWebDriver driver, UiSettings settings)
        {
            // Configure timeouts
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settings.ImplicitWait);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(settings.PageLoadTimeout);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(settings.ScriptTimeout);

            // Configure window size
            if (settings.WindowWidth > 0 && settings.WindowHeight > 0)
            {
                driver.Manage().Window.Size = new System.Drawing.Size(settings.WindowWidth, settings.WindowHeight);
            }
            else
            {
                driver.Manage().Window.Maximize();
            }

            _logger.Debug("WebDriver configured with ImplicitWait: {ImplicitWait}s, PageLoad: {PageLoad}s, Window: {Width}x{Height}",
                settings.ImplicitWait,
                settings.PageLoadTimeout,
                settings.WindowWidth,
                settings.WindowHeight);
        }
    }
} 