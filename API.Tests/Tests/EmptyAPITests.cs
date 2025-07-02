using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace API.Tests.Tests
{
    [TestFixture]
    [AllureSuite("Empty API Tests")]
    [AllureFeature("For presentation purposes only")]
    public class EmptyAPITests
    {
        [Test]
        [AllureStory("Successful Order")]
        [AllureSeverity(Allure.Net.Commons.SeverityLevel.normal)]
        [AllureTag("Smoke", "API")]
        [AllureOwner("QA Team")]
        [Description("Empty test for API presentation.")]

        public void EmpryTest()
        { 
            //
        }
    }
}
