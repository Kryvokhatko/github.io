using System;
using System.Threading.Tasks;

namespace CSTestFramework.Core.Configuration.Interfaces
{
    /// <summary>
    /// Base interface for configuration providers that defines the contract for loading and accessing configuration.
    /// </summary>
    /// <typeparam name="T">The type of configuration to provide</typeparam>
    public interface IConfigurationProvider<T> where T : class
    {
        /// <summary>
        /// Gets the current configuration instance.
        /// </summary>
        T Configuration { get; }

        /// <summary>
        /// Loads the configuration asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task LoadAsync();

        /// <summary>
        /// Reloads the configuration asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ReloadAsync();

        /// <summary>
        /// Validates the current configuration.
        /// </summary>
        /// <returns>True if the configuration is valid; otherwise, false.</returns>
        bool Validate();
    }
} 