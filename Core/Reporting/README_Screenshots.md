# Automatic Screenshot Functionality

This document explains how to use the automatic screenshot functionality in the C# Test Framework.

## Overview

The framework provides automatic screenshot capture with three configuration modes:
- **Never**: No screenshots are taken automatically
- **OnFailureOnly**: Screenshots are taken only when tests fail (default)
- **Always**: Screenshots are taken for all tests (pass and fail)

The implementation uses a **hybrid approach** that ensures both UI and API tests get valuable debugging information:
- **UI Tests**: Take actual browser screenshots using Selenium WebDriver
- **API Tests**: Take context screenshots (test data, logs, environment info) when WebDriver is not available

This approach provides comprehensive debugging capabilities for all test types without requiring browser dependencies for API tests.

## Configuration

### Screenshot Settings in appsettings.json

```json
{
  "Logging": {
    "ScreenshotMode": "OnFailureOnly",
    "ScreenshotDirectory": "screenshots",
    "ScreenshotFormat": "png",
    "IncludeScreenshotInAllure": true,
    "MaxScreenshotSize": 5242880
  }
}
```

### Configuration Options

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `ScreenshotMode` | `ScreenshotMode` | `OnFailureOnly` | When to take screenshots: `Never`, `OnFailureOnly`, `Always` |
| `ScreenshotDirectory` | `string` | `"screenshots"` | Directory to save screenshot files |
| `ScreenshotFormat` | `string` | `"png"` | File format for browser screenshots |
| `IncludeScreenshotInAllure` | `bool` | `true` | Whether to include screenshots in Allure reports |
| `MaxScreenshotSize` | `int` | `5242880` (5MB) | Maximum file size for screenshots in bytes |

## Usage

### Basic Usage

The automatic screenshot functionality is built into the `TestBase` class. Simply inherit from `TestBase` and the framework will automatically handle screenshots based on your configuration:

```csharp
[TestFixture]
public class MyTests : TestBase
{
    [Test]
    public void MyTest()
    {
        // Your test code here
        // Screenshots will be taken automatically based on configuration
        // UI tests: Browser screenshots (if WebDriver configured)
        // API tests: Context screenshots (JSON with test data)
    }
}
```

### Manual Screenshots

You can also take manual screenshots during test execution:

```csharp
[Test]
public void TestWithManualScreenshot()
{
    // Take a manual screenshot
    var screenshotPath = TakeScreenshot("Step1", "Screenshot after step 1");
    
    // Continue with test...
    
    // Take another screenshot
    var finalScreenshotPath = TakeScreenshot("Final", "Final state screenshot");
}
```

### UI Tests with WebDriver

For UI tests, you need to provide a WebDriver instance by overriding the `WebDriver` property:

```csharp
[TestFixture]
public class UiTests : TestBase
{
    private IWebDriver _webDriver;

    /// <summary>
    /// Override the WebDriver property to provide WebDriver access.
    /// </summary>
    protected override IWebDriver WebDriver => _webDriver;

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        // Set up WebDriver before calling base
        _webDriver = new ChromeDriver();
        _webDriver.Manage().Window.Maximize();
        _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        
        base.OneTimeSetUp();
    }

    [OneTimeTearDown]
    public override void OneTimeTearDown()
    {
        base.OneTimeTearDown();
        
        // Clean up WebDriver
        _webDriver?.Quit();
        _webDriver?.Dispose();
    }

    [Test]
    public void UiTestWithScreenshots()
    {
        // Navigate to application
        _webDriver.Navigate().GoToUrl("https://example.com");
        
        // Perform UI interactions
        _webDriver.FindElement(By.Id("login-button")).Click();
        
        // Take manual screenshot
        var screenshotPath = TakeScreenshot("AfterLogin", "Screenshot after login");
        
        // Continue with test...
    }
}
```

### API Tests (Context Screenshots)

For API tests, context screenshots will be taken automatically when WebDriver is not configured:

```csharp
[TestFixture]
public class ApiTests : TestBase
{
    [Test]
    public void ApiTest()
    {
        // No WebDriver configured, so context screenshots will be taken
        // These include test data, environment info, memory usage, etc.
        
        // Your API test code here
        var response = MakeApiCall();
        
        // Context screenshot will be taken automatically on failure
        Assert.That(response.StatusCode, Is.EqualTo(200));
    }
}
```

## Screenshot Types

### Browser Screenshots (UI Tests)

When WebDriver is configured, the framework takes actual browser screenshots:

- **Format**: PNG image files
- **Content**: Visual state of the browser
- **Use Case**: UI debugging, visual regression testing
- **File Extension**: `.png`
- **Technology**: Selenium WebDriver's built-in screenshot functionality

### Context Screenshots (API Tests)

When WebDriver is not configured, the framework takes context screenshots:

- **Format**: JSON files with test context data
- **Content**: Test metadata, environment info, memory usage, etc.
- **Use Case**: API test debugging, performance analysis
- **File Extension**: `.json`
- **Technology**: Serialized test context data

**Context Data Includes:**
- Test name and result
- Timestamp and environment info
- Process ID and thread ID
- Memory usage statistics
- Test method and class information
- Test description and categories
- Manual screenshot name and description (for manual screenshots)

## Screenshot Modes

### Never Mode

```json
{
  "Logging": {
    "ScreenshotMode": "Never"
  }
}
```

- No automatic screenshots are taken
- Manual screenshots still work (both browser and context)
- Useful when screenshots are not needed
- Reduces test execution time and storage usage

### OnFailureOnly Mode (Default)

```json
{
  "Logging": {
    "ScreenshotMode": "OnFailureOnly"
  }
}
```

- Screenshots are taken only when tests fail
- UI tests: Browser screenshots on failure
- API tests: Context screenshots on failure
- Helps with debugging failed tests
- Reduces storage usage and test execution time
- Recommended for most scenarios

### Always Mode

```json
{
  "Logging": {
    "ScreenshotMode": "Always"
  }
}
```

- Screenshots are taken for all tests (pass and fail)
- UI tests: Browser screenshots for all tests
- API tests: Context screenshots for all tests
- Useful for visual regression testing and complete audit trails
- Increases storage usage and test execution time
- Use sparingly due to storage and performance impact

## File Naming Convention

Screenshots are saved with the following naming convention:

### Automatic Browser Screenshots (UI Tests)
```
{safeTestName}_{testResult}_{timestamp}.{format}
```

### Automatic Context Screenshots (API Tests)
```
{safeTestName}_{testResult}_Context_{timestamp}.json
```

### Manual Screenshots
```
{safeName}_{timestamp}.{format} (UI tests)
{safeName}_Manual_{timestamp}.json (API tests)
```

Examples:
- `LoginTest_Failed_20241201_143022.png` (UI test, automatic)
- `UserRegistration_Passed_20241201_143045.png` (UI test, automatic)
- `ApiTest_Failed_Context_20241201_143100.json` (API test, automatic)
- `Step1_20241201_143100.png` (UI test, manual)
- `ApiStep_Manual_20241201_143115.json` (API test, manual)

**Note**: File names are sanitized to remove invalid characters and limited to 100 characters.

## Allure Integration

When `IncludeScreenshotInAllure` is enabled, screenshots are automatically attached to Allure reports with descriptive names and descriptions.

### Browser Screenshots in Allure

- **Name**: `Screenshot_{testResult}_{testName}`
- **Description**: `Browser screenshot for {testName} (Result: {testResult})`
- **Type**: Image attachment (`image/png`)
- **Content**: Actual browser screenshot

### Context Screenshots in Allure

- **Name**: `Context_{testResult}_{testName}`
- **Description**: `Test context data for {testName} (Result: {testResult})`
- **Type**: JSON attachment (`application/json`)
- **Content**: Test metadata and environment information

### Manual Screenshots in Allure

- **Name**: The name provided to `TakeScreenshot()`
- **Description**: The description provided to `TakeScreenshot()`
- **Type**: Image or JSON based on test type
- **Content**: Browser screenshot or context data based on WebDriver availability

## Cleanup

The framework automatically cleans up old screenshots based on the configuration. By default, screenshots older than 30 days are removed during test suite teardown.

## Error Handling

The screenshot functionality is designed to be non-intrusive:

- If no WebDriver is configured, API tests get context screenshots instead
- Screenshot failures are logged but don't cause test failures
- File size limits are enforced with warnings
- Invalid file names are automatically sanitized
- Directory creation is handled automatically
- Graceful degradation when screenshot service is unavailable

## Best Practices

1. **Use OnFailureOnly for most scenarios** - Provides good debugging information without excessive storage usage
2. **Use Always mode sparingly** - Only when visual regression testing is needed
3. **Configure appropriate file size limits** - Prevents extremely large screenshot files
4. **Set up WebDriver in UI tests** - Enable actual browser screenshots
5. **Use descriptive names for manual screenshots** - Makes debugging easier
6. **Monitor screenshot storage usage** - Clean up old screenshots regularly
7. **Override WebDriver property in UI tests** - Ensures screenshots are taken from the correct browser instance
8. **Leverage context screenshots for API tests** - Provides valuable debugging information without browser dependency
9. **Use LogTestData() for API tests** - Enhances context screenshots with test-specific data
10. **Use LogPerformanceMetrics() for performance tests** - Adds performance data to context screenshots

## Troubleshooting

### Screenshots Not Being Taken

1. Check the `ScreenshotMode` configuration
2. Verify that WebDriver is configured (for UI tests)
3. Check the logs for error messages
4. Ensure the screenshot directory is writable
5. Verify that the test is actually failing (for OnFailureOnly mode)

### Screenshots Not Appearing in Allure

1. Verify `IncludeScreenshotInAllure` is set to `true`
2. Check that Allure reporting is properly configured
3. Look for error messages in the logs
4. Ensure AllureTestContext is properly initialized

### Large Screenshot Files

1. Check the `MaxScreenshotSize` setting
2. Consider using a different image format for browser screenshots
3. Implement browser window size optimization
4. Monitor context screenshot size for API tests

### WebDriver Issues

1. Ensure WebDriver is properly initialized before test execution
2. Check that WebDriver supports screenshot functionality
3. Verify WebDriver is not disposed before screenshots are taken
4. Ensure WebDriver property is properly overridden in UI test classes

### Context Screenshot Issues

1. Context screenshots are JSON files, not images
2. They contain test metadata and environment information
3. Useful for debugging API test failures and performance issues
4. Check that NUnit TestContext is available for context data

## Example Configuration Files

### Development Environment
```json
{
  "Logging": {
    "ScreenshotMode": "OnFailureOnly",
    "ScreenshotDirectory": "screenshots",
    "ScreenshotFormat": "png",
    "IncludeScreenshotInAllure": true,
    "MaxScreenshotSize": 5242880
  }
}
```

### CI/CD Environment
```json
{
  "Logging": {
    "ScreenshotMode": "OnFailureOnly",
    "ScreenshotDirectory": "artifacts/screenshots",
    "ScreenshotFormat": "png",
    "IncludeScreenshotInAllure": true,
    "MaxScreenshotSize": 10485760
  }
}
```

### Visual Regression Testing
```json
{
  "Logging": {
    "ScreenshotMode": "Always",
    "ScreenshotDirectory": "screenshots/regression",
    "ScreenshotFormat": "png",
    "IncludeScreenshotInAllure": true,
    "MaxScreenshotSize": 5242880
  }
}
```

### API Testing Focus
```json
{
  "Logging": {
    "ScreenshotMode": "OnFailureOnly",
    "ScreenshotDirectory": "screenshots/api",
    "ScreenshotFormat": "png",
    "IncludeScreenshotInAllure": true,
    "MaxScreenshotSize": 2097152
  }
}
```

## Technical Implementation

The screenshot functionality uses a hybrid approach:

### Browser Screenshots (UI Tests)
```csharp
// Direct Selenium implementation
var screenshot = ((ITakesScreenshot)_webDriver).GetScreenshot();
var screenshotBytes = screenshot.AsByteArray;
```

### Context Screenshots (API Tests)
```csharp
// JSON context data
var contextData = new { 
    TestName, TestResult, Timestamp, Environment, 
    ProcessId, ThreadId, MemoryUsage, TestContext 
};
var contextJson = JsonConvert.SerializeObject(contextData, Formatting.Indented);
var contextBytes = Encoding.UTF8.GetBytes(contextJson);
```

### Hybrid Decision Logic
```csharp
if (_webDriver != null)
{
    // UI Test: Take actual browser screenshot
    screenshotPath = TakeBrowserScreenshot(testName, testResult);
}
else
{
    // API Test: Take context screenshot (test data, logs, etc.)
    screenshotPath = TakeContextScreenshot(testName, testResult);
}
```

This approach provides:
- **Reliability**: Uses industry-standard Selenium screenshot functionality for UI tests
- **Flexibility**: Provides context screenshots for API tests when browser screenshots aren't possible
- **Compatibility**: Works with all Selenium WebDriver implementations
- **Simplicity**: One service handles both screenshot types
- **Value**: Both UI and API tests get meaningful debugging information
- **Performance**: Minimal overhead for API tests
- **Maintainability**: Single codebase for both screenshot types

## Benefits

- **UI Tests**: Get actual browser screenshots for visual debugging
- **API Tests**: Get context screenshots with test metadata and environment info
- **Unified Approach**: Same configuration and API for both test types
- **Allure Integration**: Both screenshot types are properly integrated with Allure reports
- **Non-Intrusive**: Screenshot failures don't cause test failures
- **Configurable**: Three modes to suit different testing scenarios
- **Storage Efficient**: Context screenshots are much smaller than browser screenshots
- **Debugging Rich**: Context screenshots provide comprehensive test environment information

## Framework Integration

The screenshot functionality is fully integrated with the test framework:

- **TestBase Integration**: Automatic screenshot capture in TearDown
- **Configuration Integration**: Uses LoggingSettings for configuration
- **Logging Integration**: Comprehensive logging of screenshot operations
- **Allure Integration**: Proper attachment handling for both screenshot types
- **Error Handling**: Graceful degradation and non-intrusive operation
- **Cleanup Integration**: Automatic cleanup of old screenshots 