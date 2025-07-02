using NUnit.Framework;
using CSTestFramework.Core.Infrastructure;

//When you place a class with [SetUpFixture] in a project, NUnit knows to run its [OneTimeSetUp]
//method once before any tests in that specific assembly (or namespace) start.
//That is why there is no namespace here

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void GlobalSetupAction()
    {
        FrameworkInitializer.Initialize();
    }

    [OneTimeTearDown]
    public void GlobalTeardownAction()
    {
        FrameworkInitializer.Teardown();
    }
}