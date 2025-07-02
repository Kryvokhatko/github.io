using System;
using System.Diagnostics;
using Allure.NUnit.Attributes;
using CSTestFramework.Core.Logging;
using CSTestFramework.Core.Logging.Interfaces;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Provides helper methods for creating and managing Allure test steps.
    /// </summary>
    public static class AllureStepHelper
    {
        private static readonly ILogger _logger = TestLoggerHelper.CreateTestLogger("AllureStepHelper");

        /// <summary>
        /// Executes an action within an Allure step.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="action">The action to execute.</param>
        /// <param name="stepDescription">Optional step description.</param>
        public static void ExecuteStep(string stepName, Action action, string stepDescription = null)
        {
            var step = AllureTestContext.Current.AddStep(stepName, stepDescription);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Debug("Starting step: {StepName}", stepName);
                action();
                stopwatch.Stop();

                AllureTestContext.Current.CompleteStep(step, StepStatus.Passed);
                _logger.Debug("Step completed successfully: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AllureTestContext.Current.CompleteStep(step, StepStatus.Failed, ex.Message);
                _logger.Debug(ex, "Step failed: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Executes a function within an Allure step and returns the result.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="stepName">The step name.</param>
        /// <param name="func">The function to execute.</param>
        /// <param name="stepDescription">Optional step description.</param>
        /// <returns>The result of the function.</returns>
        public static T ExecuteStep<T>(string stepName, Func<T> func, string stepDescription = null)
        {
            var step = AllureTestContext.Current.AddStep(stepName, stepDescription);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Debug("Starting step: {StepName}", stepName);
                var result = func();
                stopwatch.Stop();

                AllureTestContext.Current.CompleteStep(step, StepStatus.Passed);
                _logger.Debug("Step completed successfully: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AllureTestContext.Current.CompleteStep(step, StepStatus.Failed, ex.Message);
                _logger.Debug(ex, "Step failed: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Executes an async action within an Allure step.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="action">The async action to execute.</param>
        /// <param name="stepDescription">Optional step description.</param>
        public static async System.Threading.Tasks.Task ExecuteStepAsync(string stepName, Func<System.Threading.Tasks.Task> action, string stepDescription = null)
        {
            var step = AllureTestContext.Current.AddStep(stepName, stepDescription);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Debug("Starting async step: {StepName}", stepName);
                await action();
                stopwatch.Stop();

                AllureTestContext.Current.CompleteStep(step, StepStatus.Passed);
                _logger.Debug("Async step completed successfully: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AllureTestContext.Current.CompleteStep(step, StepStatus.Failed, ex.Message);
                _logger.Debug(ex, "Async step failed: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Executes an async function within an Allure step and returns the result.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="stepName">The step name.</param>
        /// <param name="func">The async function to execute.</param>
        /// <param name="stepDescription">Optional step description.</param>
        /// <returns>The result of the async function.</returns>
        public static async System.Threading.Tasks.Task<T> ExecuteStepAsync<T>(string stepName, Func<System.Threading.Tasks.Task<T>> func, string stepDescription = null)
        {
            var step = AllureTestContext.Current.AddStep(stepName, stepDescription);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                _logger.Debug("Starting async step: {StepName}", stepName);
                var result = await func();
                stopwatch.Stop();

                AllureTestContext.Current.CompleteStep(step, StepStatus.Passed);
                _logger.Debug("Async step completed successfully: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                AllureTestContext.Current.CompleteStep(step, StepStatus.Failed, ex.Message);
                _logger.Debug(ex, "Async step failed: {StepName}, Duration: {Duration}ms", 
                    stepName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }

        /// <summary>
        /// Creates a step for UI interactions.
        /// </summary>
        /// <param name="action">The UI action to perform.</param>
        /// <param name="elementDescription">Description of the element being interacted with.</param>
        /// <param name="actionDescription">Description of the action being performed.</param>
        public static void ExecuteUIStep(Action action, string elementDescription, string actionDescription)
        {
            var stepName = $"{actionDescription} {elementDescription}";
            ExecuteStep(stepName, action, $"UI Interaction: {actionDescription} on {elementDescription}");
        }

        /// <summary>
        /// Creates a step for API interactions.
        /// </summary>
        /// <param name="action">The API action to perform.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="endpoint">The API endpoint.</param>
        public static void ExecuteAPIStep(Action action, string httpMethod, string endpoint)
        {
            var stepName = $"{httpMethod} {endpoint}";
            ExecuteStep(stepName, action, $"API Request: {httpMethod} {endpoint}");
        }

        /// <summary>
        /// Creates a step for API interactions with return value.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="func">The API function to execute.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="endpoint">The API endpoint.</param>
        /// <returns>The API response.</returns>
        public static T ExecuteAPIStep<T>(Func<T> func, string httpMethod, string endpoint)
        {
            var stepName = $"{httpMethod} {endpoint}";
            return ExecuteStep(stepName, func, $"API Request: {httpMethod} {endpoint}");
        }

        /// <summary>
        /// Creates a step for verification/assertion operations.
        /// </summary>
        /// <param name="action">The verification action to perform.</param>
        /// <param name="verificationDescription">Description of what is being verified.</param>
        public static void ExecuteVerificationStep(Action action, string verificationDescription)
        {
            var stepName = $"Verify {verificationDescription}";
            ExecuteStep(stepName, action, $"Verification: {verificationDescription}");
        }

        /// <summary>
        /// Creates a step for data preparation operations.
        /// </summary>
        /// <param name="action">The data preparation action to perform.</param>
        /// <param name="dataDescription">Description of the data being prepared.</param>
        public static void ExecuteDataPreparationStep(Action action, string dataDescription)
        {
            var stepName = $"Prepare {dataDescription}";
            ExecuteStep(stepName, action, $"Data Preparation: {dataDescription}");
        }
    }
} 