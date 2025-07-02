namespace CSTestFramework.Core.Configuration.Interfaces
{
    /// <summary>
    /// Interface for environment-specific configuration settings.
    /// </summary>
    public interface IEnvironmentConfiguration
    {
        /// <summary>
        /// Gets the name of the current environment (e.g., "Development", "Test", "Production").
        /// </summary>
        string EnvironmentName { get; }

        /// <summary>
        /// Gets a value indicating whether the current environment is a development environment.
        /// </summary>
        bool IsDevelopment { get; }

        /// <summary>
        /// Gets a value indicating whether the current environment is a test environment.
        /// </summary>
        bool IsTest { get; }

        /// <summary>
        /// Gets a value indicating whether the current environment is a production environment.
        /// </summary>
        bool IsProduction { get; }

        /// <summary>
        /// Gets a value indicating whether debug mode is enabled.
        /// </summary>
        bool IsDebug { get; }
    }
} 