using System;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MicrosoftSolutions.IoT.Edge.OpcToDtdl.Options;

[assembly: FunctionsStartup(typeof(MicrosoftSolutions.IoT.Edge.OpcToDtdl.Startup))]
namespace MicrosoftSolutions.IoT.Edge.OpcToDtdl
{
    /// <summary>
    ///  This class manages DI service registration for this Azure Function App
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        ///  Configures DI Services and configuration for this Function App
        /// </summary>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            
            // configurationBuilder.AddJsonFile("local.settings.json");
            // configurationBuilder.AddEnvironmentVariables();
            
            var configuration = configurationBuilder.Build();
            builder.Services.Configure<OpcToDtdlOptions>(configuration);
        }
    }
}