using System;
using System.Threading;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.UI.PageObjects.Base;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CSTestFramework.UI.PageObjects
{
    public class ProductItemPage : BasePage
    {

        private By CartButton => By.CssSelector("#cart");
        private By AddToCartButton => By.CssSelector("button[name='add_cart_product']");
        private By QuantityInput => By.CssSelector("input[name='quantity']");
        private By CartBadge => By.CssSelector("div[class=\"badge quantity\"]");

        public ProductItemPage(IWebDriver driver, ILogger logger) : base(driver, logger) { }

        public void SetQuantity(int quantity)
        {
            Logger.Debug($"Setting quantity to {quantity}.");
            var input = Driver.FindElement(QuantityInput);
            input.Clear();
            input.SendKeys(quantity.ToString());
        }

        public void AddToCart()
        {
            Logger.Debug("Clicking Add To Cart.");
            Driver.FindElement(AddToCartButton).Click();
        }

        public CartPage GoToCart()
        {
            try
            {
                var cartBadge = new WebDriverWait(Driver, TimeSpan.FromSeconds(3)).Until(
                    ExpectedConditions.ElementIsVisible(CartBadge));
                if (cartBadge.Displayed)
                {
                    Logger.Debug("Navigating to Cart.");
                    Driver.FindElement(CartButton).Click();   
                }
            }
            catch (WebDriverTimeoutException)
            {
                Logger.Debug("Cart badge was not visible within the timeout period.");
                return null;
            }
            return new CartPage(Driver, Logger);
        }
    }
} 