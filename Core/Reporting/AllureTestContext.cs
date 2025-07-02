using System;
using System.Collections.Generic;
using System.Threading;
using Allure.NUnit.Attributes;
using CSTestFramework.Core.Logging.Interfaces;
using CSTestFramework.Core.Logging;

namespace CSTestFramework.Core.Reporting
{
    /// <summary>
    /// Manages test context and metadata for Allure reporting.
    /// </summary>
    public class AllureTestContext
    {
        private static readonly AsyncLocal<AllureTestContext> _current = new AsyncLocal<AllureTestContext>();
        private readonly List<AllureStep> _steps = new List<AllureStep>();
        private readonly List<AllureAttachment> _attachments = new List<AllureAttachment>();
        private readonly ILogger _logger;

        /// <summary>
        /// Gets the current test context.
        /// </summary>
        public static AllureTestContext Current
        {
            get
            {
                if (_current.Value == null)
                {
                    _current.Value = new AllureTestContext();
                }
                return _current.Value;
            }
        }

        /// <summary>
        /// Gets the current test name.
        /// </summary>
        public string TestName { get; private set; }

        /// <summary>
        /// Gets the current test description.
        /// </summary>
        public string TestDescription { get; private set; }

        /// <summary>
        /// Gets the test severity level.
        /// </summary>
        public SeverityLevel Severity { get; private set; } = SeverityLevel.Normal;

        /// <summary>
        /// Gets the test category.
        /// </summary>
        public string Category { get; private set; } = "General";

        /// <summary>
        /// Gets the collection of test steps.
        /// </summary>
        public IReadOnlyList<AllureStep> Steps => _steps.AsReadOnly();

        /// <summary>
        /// Gets the collection of test attachments.
        /// </summary>
        public IReadOnlyList<AllureAttachment> Attachments => _attachments.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="AllureTestContext"/> class.
        /// </summary>
        public AllureTestContext()
        {
            _logger = TestLoggerHelper.CreateTestLogger("AllureTestContext");
        }

        /// <summary>
        /// Sets the current test information.
        /// </summary>
        /// <param name="testName">The test name.</param>
        /// <param name="description">The test description.</param>
        /// <param name="severity">The test severity level.</param>
        /// <param name="category">The test category.</param>
        public void SetTestInfo(string testName, string description = null, SeverityLevel severity = SeverityLevel.Normal, string category = "General")
        {
            TestName = testName;
            TestDescription = description;
            Severity = severity;
            Category = category;

            _logger.Debug("Test context set: {TestName}, Category: {Category}, Severity: {Severity}", 
                testName, category, severity);
        }

        /// <summary>
        /// Adds a step to the current test.
        /// </summary>
        /// <param name="stepName">The step name.</param>
        /// <param name="stepDescription">The step description.</param>
        /// <returns>The created step.</returns>
        public AllureStep AddStep(string stepName, string stepDescription = null)
        {
            var step = new AllureStep
            {
                Name = stepName,
                Description = stepDescription,
                StartTime = DateTime.UtcNow
            };

            _steps.Add(step);
            _logger.Debug("Step added: {StepName}", stepName);

            return step;
        }

        /// <summary>
        /// Completes a step with the specified status.
        /// </summary>
        /// <param name="step">The step to complete.</param>
        /// <param name="status">The step status.</param>
        /// <param name="errorMessage">The error message if the step failed.</param>
        public void CompleteStep(AllureStep step, StepStatus status = StepStatus.Passed, string errorMessage = null)
        {
            if (step == null)
                return;

            step.EndTime = DateTime.UtcNow;
            step.Status = status;
            step.ErrorMessage = errorMessage;

            var duration = step.EndTime.Value - step.StartTime;
            _logger.Debug("Step completed: {StepName}, Status: {Status}, Duration: {Duration}ms", 
                step.Name, status, duration.TotalMilliseconds);
        }

        /// <summary>
        /// Adds an attachment to the current test.
        /// </summary>
        /// <param name="name">The attachment name.</param>
        /// <param name="content">The attachment content.</param>
        /// <param name="type">The attachment type.</param>
        /// <param name="extension">The file extension.</param>
        public void AddAttachment(string name, byte[] content, string type, string extension)
        {
            var attachment = new AllureAttachment
            {
                Name = name,
                Content = content,
                Type = type,
                Extension = extension
            };

            _attachments.Add(attachment);
            _logger.Debug("Attachment added: {AttachmentName}, Type: {Type}, Size: {Size} bytes", 
                name, type, content?.Length ?? 0);
        }

        /// <summary>
        /// Adds a text attachment to the current test.
        /// </summary>
        /// <param name="name">The attachment name.</param>
        /// <param name="content">The text content.</param>
        /// <param name="extension">The file extension.</param>
        public void AddTextAttachment(string name, string content, string extension = "txt")
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            AddAttachment(name, bytes, "text/plain", extension);
        }

        /// <summary>
        /// Clears the current test context.
        /// </summary>
        public void Clear()
        {
            TestName = null;
            TestDescription = null;
            Severity = SeverityLevel.Normal;
            Category = "General";
            _steps.Clear();
            _attachments.Clear();

            _logger.Debug("Test context cleared");
        }

        /// <summary>
        /// Resets the current test context.
        /// </summary>
        public static void Reset()
        {
            _current.Value = null;
        }
    }

    /// <summary>
    /// Represents an Allure test step.
    /// </summary>
    public class AllureStep
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? Duration 
        { 
            get 
            { 
                return EndTime.HasValue ? EndTime.Value - StartTime : (TimeSpan?)null; 
            } 
        }
        public StepStatus Status { get; set; } = StepStatus.Passed;
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Represents an Allure attachment.
    /// </summary>
    public class AllureAttachment
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
    }

    /// <summary>
    /// Represents the status of a test step.
    /// </summary>
    public enum StepStatus
    {
        Passed,
        Failed,
        Skipped,
        Broken
    }
} 