using System;

namespace CSTestFramework.Core.Configuration.Interfaces
{
    /// <summary>
    /// Interface for API test configuration settings.
    /// </summary>
    public interface IApiConfiguration
    {
        /// <summary>
        /// Gets the base URL for the API.
        /// </summary>
        string ApiUrl { get; }

        /// <summary>
        /// Gets the timeout in seconds for API requests.
        /// </summary>
        int Timeout { get; }

        /// <summary>
        /// Gets the number of retry attempts for failed API requests.
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// Gets the delay in milliseconds between retry attempts.
        /// </summary>
        int RetryDelay { get; }

        /// <summary>
        /// Gets the API version.
        /// </summary>
        string ApiVersion { get; }

        /// <summary>
        /// Gets the authentication token for API requests.
        /// </summary>
        string AuthToken { get; }

        /// <summary>
        /// Gets a value indicating whether to log API requests and responses.
        /// </summary>
        bool EnableRequestLogging { get; }

        /// <summary>
        /// Gets the maximum number of concurrent API requests.
        /// </summary>
        int MaxConcurrentRequests { get; }
    }
} 