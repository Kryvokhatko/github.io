using OpenQA.Selenium;
using SeleniumExtras.PageObjects;
using CSTestFramework.Core.Logging.Interfaces;
using OpenQA.Selenium.Support.UI;
using System;

namespace CSTestFramework.UI.PageObjects.Base
{
    /// <summary>
    /// Represents the base class for all Page Objects.
    /// It provides shared functionality like WebDriver access and page factory initialization.
    /// </summary>
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the "BasePage" class.
        /// </summary>
        /// <param name="driver">The WebDriver instance to be used by the page.</param>
        /// <param name="logger">The logger instance for logging page activities.</param>
        protected BasePage(IWebDriver driver, ILogger logger)
        {
            Driver = driver;
            Logger = logger;
        }

        /// <summary>
        /// Gets the title of the current page.
        /// </summary>
        /// <returns>The page title as a string.</returns>
        public string GetPageTitle()
        {
            Logger.Debug("Getting page title.");
            return Driver.Title;
        }

        /// <summary>
        /// Gets the URL of the current page.
        /// </summary>
        /// <returns>The page URL as a string.</returns>
        public string GetPageUrl()
        {
            Logger.Debug("Getting page URL.");
            return Driver.Url;
        }

        /// <summary>
        /// Navigates the browser to a specified URL.
        /// </summary>
        /// <param name="url">The URL to navigate to.</param>
        public void NavigateTo(string url)
        {
            Logger.Debug("Navigating to URL: {Url}", url);
            Driver.Navigate().GoToUrl(url);
        }
    }
} 