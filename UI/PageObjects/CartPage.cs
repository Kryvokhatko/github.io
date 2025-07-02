using OpenQA.Selenium;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.UI.PageObjects.Base;

namespace CSTestFramework.UI.PageObjects
{
    public class CartPage : BasePage
    {
        private By TermsCheckbox => By.CssSelector("input[name='terms_agreed']");
        private By ConfirmOrderButton => By.CssSelector("button[name='confirm_order']");

        public CartPage(IWebDriver driver, ILogger logger) : base(driver, logger) { }

        public void AcceptPrivacyPolicy()
        {
            Logger.Debug("Accepting privacy policy.");
            // The checkbox is inside of a form, ensure it's clickable
            var checkbox = Driver.FindElement(TermsCheckbox);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", checkbox);
            checkbox.Click();
        }

        public OrderConfirmationPage ConfirmOrder()
        {
            Logger.Debug("Confirming order.");
            Driver.FindElement(ConfirmOrderButton).Click();
            return new OrderConfirmationPage(Driver, Logger);
        }
    }
} 