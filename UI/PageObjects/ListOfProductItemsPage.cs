using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.UI.PageObjects.Base;
using OpenQA.Selenium;

namespace CSTestFramework.UI.PageObjects
{
    public class ListOfProductItemsPage : BasePage
    {
        private By RedDuckProduct => By.CssSelector("[alt=\"Red Duck\"]");

        public ListOfProductItemsPage(IWebDriver driver, ILogger logger) : base(driver, logger) { }

        public ProductItemPage ClickRedDuck()
        {
            Logger.Debug("Clicking Red Duck product.");
            Driver.FindElement(RedDuckProduct).Click();
            return new ProductItemPage(Driver, Logger);
        }
    }
}
