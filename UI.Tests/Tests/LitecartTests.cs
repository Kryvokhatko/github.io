using NUnit.Framework;
using CSTestFramework.Core.Reporting;
using CSTestFramework.UI.PageObjects;
using Allure.NUnit.Attributes;

namespace CSTestFramework.UI.Tests
{
    [TestFixture]
    [AllureSuite("E-Commerce Tests")]
    [AllureFeature("Order Placement")]
    public class LitecartTests : UiTestBase
    {
        [Test(Description = "Automates the process of placing an order for a Red Duck item.")]
        [AllureStory("Successful Order")]
        [AllureTag("Smoke", "E2E")]
        public void PlaceOrderForRedDuck_Success()
        {
            var homePage = new HomePage(WebDriver, Logger);

            AllureStepHelper.ExecuteStep("Navigate to Home Page", () =>
            {
                homePage.GoToHomePage();                
            });

            AllureStepHelper.ExecuteStep("Make Sign In and accept Change Password and cookies alerts ", () =>
            {
                homePage.ClickSignInDropdown();
                homePage.ClickSignInButtonInDropdown();
                homePage.AcceptChangePasswordAlertIfPresent();
                homePage.AcceptCookiesIfPresent();
            });

            var listOfProductItemsPage = AllureStepHelper.ExecuteStep("Navigate to Red Duck product item page", () =>
            {
                homePage.ClickCategoriesDropdown();
                return homePage.ClickRubberDucks();
            });
            
            var productItemPage = AllureStepHelper.ExecuteStep("Select Red Duck", () =>
            {
                return listOfProductItemsPage.ClickRedDuck();
            });

            AllureStepHelper.ExecuteStep("Add product items to cart", () =>
            {
                productItemPage.SetQuantity(2);
                productItemPage.AddToCart();
            });

            var cartPage = AllureStepHelper.ExecuteStep("Navigate to Cart", () =>
            {
                return productItemPage.GoToCart();
            });

            var orderConfirmationPage = AllureStepHelper.ExecuteStep("Confirm Order", () =>
            {
                cartPage.AcceptPrivacyPolicy();
                return cartPage.ConfirmOrder();
            });

            AllureStepHelper.ExecuteStep("Verify Order Confirmation", () =>
            {
                Assert.That(orderConfirmationPage.GetOrderConfirmationTitle().Contains("was completed successfully!"));            
            });
        }
    }
} 