using System;
using NUnit.Framework;
using CSTestFramework.Core.Configuration;
using CSTestFramework.Core.Logging;
using CSTestFramework.Core.Reporting;
using CSTestFramework.Core.Configuration.Models;

namespace CSTestFramework.Core.Infrastructure
{
    /// <summary>
    /// Provides centralized, one-time setup and teardown logic for the entire test run.
    /// </summary>
    public static class FrameworkInitializer
    {
        private static bool _isInitialized = false;

        /// <summary>
        /// Initializes framework components like logging and reporting.
        /// Should be called from a [OneTimeSetUp] in a [SetUpFixture].
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialized) return;

            try
            {
                var configManager = ConfigurationManager.Instance;
                var loggingConfig = configManager.AppSettings.Logging;
                
                LoggerFactory.Initialize(loggingConfig);
                var logger = LoggerFactory.CreateLogger(nameof(FrameworkInitializer));

                AllureManager.Initialize(loggingConfig, logger);

                var environmentInfo = AllureManager.BuildEnvironmentInfo();
                AllureManager.AddEnvironmentInfo(environmentInfo);
                
                logger.Debug("Framework global setup completed successfully.");
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical failure during framework initialization: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Cleans up framework resources.
        /// Should be called from a [OneTimeTearDown] in a [SetUpFixture].
        /// </summary>
        public static void Teardown()
        {
            try
            {
                var logger = LoggerFactory.CreateLogger(nameof(FrameworkInitializer));
                logger.Debug("Starting framework global teardown...");
                
                AllureManager.GenerateReport();
                AllureManager.Dispose();
                LoggerFactory.Dispose();

                logger.Debug("Framework global teardown completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical failure during framework teardown: {ex}");
            }
        }
    }
} 