# CSTestFramework

A comprehensive C# test automation framework with Selenium WebDriver integration and Allure reporting.

## Features

- Modular architecture with separation of concerns
- Selenium WebDriver integration for UI testing
- API testing capabilities
- Allure reporting for rich test results visualization
- Logging with Serilog
- Configuration management
- Page Object Model implementation

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or other compatible IDE
- Chrome browser (for UI tests)

### Installation

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Build the solution

### Running Tests

To run tests, use the Test Explorer in Visual Studio or run the following command:

```
dotnet test
```

## Allure Reporting

### Recent Fixes for GitHub Actions Integration

We've made several improvements to fix issues with Allure reporting in GitHub Actions:

1. **Fixed File Access Issues**:
   - Added retry logic in `AllureManager.cs` to handle file access conflicts
   - Implemented safe file operations with proper error handling
   - Added delays between test runs to ensure resources are released

2. **Improved GitHub Actions Workflow**:
   - Created a more robust `test-report.yml` workflow file
   - Separated API and UI test execution to prevent conflicts
   - Added proper environment setup for Allure command-line tools
   - Implemented better artifact handling and GitHub Pages deployment

3. **Updated Allure Configuration**:
   - Aligned all configuration files (main and templates)
   - Updated URLs and links to point to the actual repository
   - Added proper environment information

### Running Tests with Allure Reporting

To generate Allure reports locally:

1. Run tests using the NUnit test runner
2. Allure results will be generated in the `allure-results` directory
3. Generate the HTML report using the Allure command-line tool:

```
allure generate allure-results --clean -o allure-report
allure open allure-report
```

The GitHub Actions workflow automatically publishes the Allure report to GitHub Pages at: https://kryvokhatko.github.io/github.io/

## Project Structure

- **API** - API client library
- **API.Tests** - API test cases
- **Common** - Shared utilities and helpers
- **Core** - Framework core functionality
  - **Configuration** - Configuration management
  - **Logging** - Logging infrastructure
  - **Reporting** - Test reporting including Allure integration
- **UI** - UI automation components
  - **PageObjects** - Page Object Model implementations
- **UI.Tests** - UI test cases

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Create a Pull Request
