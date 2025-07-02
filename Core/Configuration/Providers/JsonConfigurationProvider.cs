using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSTestFramework.Core.Configuration.Interfaces;
using CSTestFramework.Core.Configuration.Models;
using Microsoft.Extensions.Configuration;

namespace CSTestFramework.Core.Configuration.Providers
{
    /// <summary>
    /// Provides configuration settings loaded from JSON files.
    /// </summary>
    public class JsonConfigurationProvider : IConfigurationProvider<AppSettings>
    {
        private IConfigurationRoot _configurationRoot;
        public AppSettings Configuration { get; private set; }

        public JsonConfigurationProvider()
        {
            // Determine the environment name (e.g., "Development", "Production")
            // This can be set via an environment variable, e.g., ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(); // For environment variable overrides

            _configurationRoot = builder.Build();

            // Load configuration immediately
            Configuration = new AppSettings();
            _configurationRoot.Bind(Configuration);
        }

        public Task LoadAsync()
        {
            // This can now be a simple refresh if needed, but the initial load is done in constructor
            _configurationRoot.Reload();
            Configuration = new AppSettings();
            _configurationRoot.Bind(Configuration);
            return Task.CompletedTask;
        }

        public Task ReloadAsync()
        {
            _configurationRoot.Reload();
            Configuration = new AppSettings();
            _configurationRoot.Bind(Configuration);
            return Task.CompletedTask;
        }

        public bool Validate()
        {
            // Simple validation: check if fundamental sections are present
            return Configuration != null &&
                   Configuration.Environment != null &&
                   Configuration.Api != null &&
                   Configuration.Ui != null;
        }
    }

}
