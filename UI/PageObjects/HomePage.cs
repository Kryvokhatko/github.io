using System;
using System.Collections.Generic;
using System.Linq;
using CSTestFramework.Core.Configuration;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.UI.PageObjects.Base;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CSTestFramework.UI.PageObjects
{
    /// <summary>
    /// Page object for the LiteCart Home Page (https://demo.litecart.net/)
    /// </summary>
    public class HomePage : BasePage
    {
        private const string HomeUrl = "https://demo.litecart.net/";

        // --- Locators as By properties ---
        private By Logo => By.CssSelector(".logotype");        
        private By SearchField => By.CssSelector("[name=\"query\"]");        
        private By Cart => By.CssSelector("#cart");        
        private By MainNavLinks => By.CssSelector("nav#site-menu a");
        private By SignInDropdown => By.CssSelector(".nav-item.account.dropdown");
        //This button available inside dropdown SinIn
        private By SignInButtonInDropdown => By.CssSelector("button[class=\"btn btn-default\"][name=\"login\"]");
        private By CookieNotice => By.CssSelector("div#box-cookie-notice");
        private By AcceptCookiesButton => By.CssSelector("button[name='accept_cookies']");
        private By CategoriesDropdown => By.CssSelector(".nav-item.categories.dropdown");
        private By RubberDucksInDropdown => By.CssSelector(".nav-link[href=\"https://demo.litecart.net/rubber-ducks-c-1/\"]");
        private By ManufacturersDropdown => By.CssSelector(".nav-item.manufacturers.dropdown");
        private By AcmeCorpInDropdown => By.CssSelector("a.nav-link[href='https://demo.litecart.net/acme-corp-m-1/']");
        private By InformationDropdown => By.CssSelector(".nav-item.information.dropdown");
        private By AboutUsInDropDown => By.CssSelector("a.nav-link[href='https://demo.litecart.net/about-us-i-1']");
        private By CustomerService => By.CssSelector(".nav-item.customer-service");
        
        // --- Constructor ---
        public HomePage(IWebDriver driver, ILogger logger) : base(driver, logger.ForContext(nameof(HomePage))) { }

        // --- Actions ---
        public void GoToHomePage()
        {
            Driver.Navigate().GoToUrl(ConfigurationManager.Instance.AppSettings.Ui.ApplicationUrl);
        }

        public bool IsLogoDisplayed()
        {
            Logger.Debug("Checking if logo is displayed on the home page.");
            return Driver.FindElement(Logo).Displayed;
        }

        public void ClickSignInDropdown()
        {
            Logger.Debug("Clicking the Sign In dropdown.");
            Driver.FindElement(SignInDropdown).Click();
        }
        
        public void ClickSignInButtonInDropdown()
        {
            Logger.Debug("Clicking Sign In button in dropdown.");
            Driver.FindElement(SignInButtonInDropdown).Click();
        }

        public void AcceptChangePasswordAlertIfPresent(int timeoutInSeconds = 2)
        {
            try
            {
                var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.Until(ExpectedConditions.AlertIsPresent());
                var alert = Driver.SwitchTo().Alert();
                Logger.Debug("Alert found with text: {AlertText}", alert.Text);
                if (alert.Text.Contains("Change your password"))
                {
                    Logger.Debug("Accepting 'Change your password' alert.");
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }               
            }
            catch (NoAlertPresentException)
            {
                // Ignore if Alert was not present
                Logger.Debug("Change Password alert was not present.");
            }
            catch (WebDriverTimeoutException)
            {
                // Ignore if Alert was not present
                Logger.Debug("Change Password alert was not present within the timeout period.");
            }
        }

        public void AcceptCookiesIfPresent()
        {
            try
            {
                var acceptButton = new WebDriverWait(Driver, TimeSpan.FromSeconds(5))
                    .Until(ExpectedConditions.ElementToBeClickable(AcceptCookiesButton));
                Logger.Debug("Accepting cookies.");
                acceptButton.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Ignore if Cookie notice was not present
                Logger.Debug("Cookie notice was not present within the timeout period.");
            }
        }

        public void ClickCategoriesDropdown()
        {
            Logger.Debug("Clicking Categories dropdown.");
            Driver.FindElement(CategoriesDropdown).Click();
        }

        public ListOfProductItemsPage ClickRubberDucks()
        {
            Logger.Debug("Clicking Rubber Ducks link.");
            Driver.FindElement(RubberDucksInDropdown).Click();
            return new ListOfProductItemsPage(Driver, Logger);
        }

        public List<string> GetMainNavLinksText()
        {
            Logger.Debug("Getting main navigation link texts.");
            return Driver.FindElements(MainNavLinks).Select(e => e.Text.Trim()).ToList();
        }
    }
} 