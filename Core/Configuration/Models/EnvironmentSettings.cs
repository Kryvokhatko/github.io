using CSTestFramework.Core.Configuration.Interfaces;

namespace CSTestFramework.Core.Configuration.Models
{
    /// <summary>
    /// Represents environment-specific configuration settings.
    /// </summary>
    public class EnvironmentSettings : IEnvironmentConfiguration
    {
        public string EnvironmentName { get; set; }
        public bool IsDevelopment { get; set; }
        public bool IsTest { get; set; }
        public bool IsProduction { get; set; }
        public bool IsDebug { get; set; }

        public EnvironmentSettings()
        {
            // Set default values if needed
            EnvironmentName = "Development";
            IsDevelopment = true;
            IsTest = false;
            IsProduction = false;
            IsDebug = true;
        }
    }
}