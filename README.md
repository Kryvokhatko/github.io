<<<<<<< HEAD
# CSTestFramework

A comprehensive C# test automation framework with Allure reporting integration.

## Features

- NUnit test framework integration
- Allure reporting with detailed test execution information
- Screenshot capture and attachment
- Structured logging
- Configuration management
- Support for both UI and API testing

## Test Report

The latest test execution report is available at: https://kryvokhatko.github.io/github.io

## Project Structure

```
CSTestFramework/
├── API/                 # API testing project
├── API.Tests/           # API test cases
├── Common/             # Shared utilities
├── Core/               # Framework core functionality
├── UI/                 # UI testing project
├── UI.Tests/           # UI test cases
└── config/             # Configuration files
```

## Configuration

The framework uses several configuration files:
- `allureConfig.json` - Allure reporting configuration
- `appsettings.json` - Application settings
- `appsettings.Development.json` - Development environment settings

## Running Tests

1. Clone the repository
2. Restore NuGet packages
3. Build the solution
4. Run tests using NUnit console or Visual Studio Test Explorer

## Allure Report Generation

The Allure report is automatically generated and published to GitHub Pages after each push to the main branch.
You can also generate the report locally using:

```powershell
allure generate allure-results -o allure-report --clean
allure open allure-report
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request 
=======
# github.io
>>>>>>> 3e4db88e4a31999513f9a6b9b39c60ef0272107d
