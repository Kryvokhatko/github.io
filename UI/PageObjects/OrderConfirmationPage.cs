using OpenQA.Selenium;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.UI.PageObjects.Base;
using OpenQA.Selenium.Support.UI;
using System;
using SeleniumExtras.WaitHelpers;

namespace CSTestFramework.UI.PageObjects
{
    public class OrderConfirmationPage : BasePage
    {
        private By CardTitle => By.CssSelector("h1.card-title");

        public OrderConfirmationPage(IWebDriver driver, ILogger logger) : base(driver, logger) { }

        public string GetOrderConfirmationTitle()
        {
            try
            {
                var titleElement = new WebDriverWait(Driver, TimeSpan.FromSeconds(10)).Until(
                    ExpectedConditions.ElementIsVisible(CardTitle));
                var title = titleElement.Text;
                Logger.Debug("Order confirmation title: {Title}", title);
                return title;
            }
            catch (WebDriverTimeoutException)
            {
                Logger.Debug("Order confirmation title was not visible within the timeout period.");
                return null;
            }
        }
    }
} 