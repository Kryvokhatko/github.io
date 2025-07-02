using System;
using System.Threading.Tasks;
using CSTestFramework.Core.Configuration.Interfaces;
using CSTestFramework.Core.Configuration.Models;
using CSTestFramework.Core.Configuration.Providers;

namespace CSTestFramework.Core.Configuration
{
    /// <summary>
    /// Manages the loading and access of application configuration settings.
    /// This is a singleton class to ensure a single source of truth for configuration.
    /// </summary>
    public sealed class ConfigurationManager
    {
        private static readonly Lazy<ConfigurationManager> _lazyInstance =
            new Lazy<ConfigurationManager>(() => new ConfigurationManager());

        readonly IConfigurationProvider<AppSettings> _configurationProvider;

        /// <summary>
        /// Gets the singleton instance of the ConfigurationManager.
        /// </summary>
        public static ConfigurationManager Instance => _lazyInstance.Value;

        /// <summary>
        /// Gets the loaded application settings.
        /// </summary>
        public AppSettings AppSettings => _configurationProvider.Configuration;

        // Private constructor to enforce singleton pattern
        private ConfigurationManager()
        {
            // Initialize with our JSON/Environment Variables provider
            _configurationProvider = new JsonConfigurationProvider();
            
            // Validate configuration on creation
            if (!Validate())
            {
                throw new InvalidOperationException("Application configuration is invalid. Please check appsettings.json and environment variables.");
            }
        }

        /// <summary>
        /// Validates the loaded configuration.
        /// </summary>
        /// <returns>True if the configuration is valid, otherwise false.</returns>
        public bool Validate()
        {
            return _configurationProvider.Validate();
        }

        /// <summary>
        /// Reloads the application settings asynchronously. Useful for dynamic configuration changes.
        /// </summary>
        /// <returns>A task representing the asynchronous reload operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the reloaded configuration is invalid.</exception>
        public async Task ReloadAsync()
        {
            await _configurationProvider.ReloadAsync();
            if (!Validate())
            {
                throw new InvalidOperationException("Application configuration reloaded and found to be invalid.");
            }
        }
    }
} 