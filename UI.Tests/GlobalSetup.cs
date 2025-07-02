using NUnit.Framework;
using CSTestFramework.Core.Infrastructure;

//When you place a class with [SetUpFixture] in a project, NUnit knows to run its [OneTimeSetUp]
//method once before any tests in that specific assembly (or namespace) start.
//That is why there is no namespace here

[SetUpFixture]
public class GlobalSetup
{
    //This ensures that global setup and teardown actions run only once for the entire test suite,
    //which is the correct way to handle this for parallel execution.

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
