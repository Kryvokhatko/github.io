using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using CSTestFramework.Core.Reporting;
using CSTestFramework.Core.Configuration;

namespace CSTestFramework.Core.Reporting
{
    public enum BrowserType
    {
        Chrome,
        Firefox,
        Edge
        // Add more browsers as needed
    }

    public abstract class UiTestBase : TestBase
    {
        private IWebDriver _webDriver;
        protected virtual BrowserType Browser => BrowserType.Chrome;
        protected override IWebDriver WebDriver => _webDriver;

        [SetUp]
        public override void SetUp()
        {
            _webDriver = WebDriverFactory.Create(Browser);
            // Get browser version and report to Allure
            string browserName = Browser.ToString();
            string browserVersion = "Unknown";
            try{
                if (_webDriver is RemoteWebDriver remoteDriver)
                {
                    // "browserVersion" is standard in W3C, fallback to "version" for legacy
                    browserVersion = remoteDriver.Capabilities.GetCapability("browserVersion")?.ToString() ??
                    remoteDriver.Capabilities.GetCapability("version")?.ToString() ??
                    "Unknown";
                }
            }
            catch (Exception ex)
            {
                Logger?.Debug(ex, "Failed to get browser version");
            }

            var environmentInfo = AllureManager.BuildEnvironmentInfo(_webDriver);
            AllureManager.AddEnvironmentInfo(environmentInfo); // Important for Allure reporting

            AllureStepHelper.ExecuteStep(
                "Browser/Environment Setup",
                () =>
                {
                    Logger?.Debug("WebDriver initialized: {Browser} {Version} on {OS}", environmentInfo.Browser, environmentInfo.BrowserVersion, environmentInfo.OperatingSystem);
                    AllureAttachmentHelper.AddJsonAttachment(environmentInfo, "Environment Info");
                },
                $"WebDriver: {environmentInfo.Browser} {environmentInfo.BrowserVersion}, OS: {environmentInfo.OperatingSystem}, BaseUrl: {environmentInfo.BaseUrl}"
            );
            
            base.SetUp();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            _webDriver?.Quit();
            _webDriver?.Dispose();
            _webDriver = null;
        }
    }
} 