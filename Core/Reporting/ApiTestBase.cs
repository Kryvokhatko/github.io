using NUnit.Framework;
using RestSharp;
using CSTestFramework.Core.Reporting;
using CSTestFramework.Core.Configuration;

namespace CSTestFramework.Core.Reporting
{
    public abstract class ApiTestBase : TestBase
    {
        protected RestClient ApiClient { get; private set; }
        protected string AuthToken { get; private set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            var apiSettings = ConfigurationManager.Instance.AppSettings.Api;
            ApiClient = new RestClient(apiSettings.ApiUrl);
            AuthToken = apiSettings.AuthToken;
        }

        // Add API-specific helpers here (e.g., SendRequest, LogRequest, etc.)
    }
} 