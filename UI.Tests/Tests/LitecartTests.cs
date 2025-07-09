using NUnit.Framework;
using CSTestFramework.Core.Reporting;
using CSTestFramework.UI.PageObjects;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using Allure.NUnit;

namespace CSTestFramework.UI.Tests
{
    [TestFixture]
    [AllureNUnit] //Class atribute for Allure reports
    [AllureSuite("Suite: Positive e-2-e tests")]
    [AllureFeature("Feature: Order Placement")]
    [AllureEpic("Epic: Web site to sell ducks")]
    public class LitecartTests : UiTestBase
    {
        [Test]
        [AllureStory("Story: Successful Order")]
        [AllureName("PlaceOrderForRedDuck_Success")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.normal)]
        [AllureTag("Smoke", "E2E")]
        [AllureOwner("QA Team")]
        [Description("Automates the process of placing an order for a Red Duck item.")]
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

        [Test]
        [AllureStory("Story: Successful Order")]
        [AllureName("Empty UI Test Success")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.blocker)]
        [AllureTag("Smoke", "E2E")]
        [AllureOwner("QA Team")]
        [Description("Automates the process of anything.")]
        public void EmptyPositiveUITest()
        {
            AllureStepHelper.ExecuteStep("Verify that 1 = 1", () =>
            {
                Assert.That(1 == 1);
            });
        }

        [Test]
        [AllureStory("Story: Negative Order")]
        [AllureName("Empty UI Test Negative")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.blocker)]
        [AllureTag("Smoke", "E2E")]
        [AllureOwner("QA Team")]
        [Description("Automates the process of anything.")]
        public void EmptyNegativeUITest()
        {
            AllureStepHelper.ExecuteStep("Verify that 1 = 2", () =>
            {
                Assert.That(1 == 2);
            });
        }

    }
} 