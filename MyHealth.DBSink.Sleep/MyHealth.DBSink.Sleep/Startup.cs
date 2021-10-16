using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.DBSink.Sleep;
using MyHealth.DBSink.Sleep.Functions;
using MyHealth.DBSink.Sleep.Repository;
using MyHealth.DBSink.Sleep.Repository.Interfaces;
using MyHealth.DBSink.Sleep.Services;
using MyHealth.DBSink.Sleep.Services.Interfaces;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyHealth.DBSink.Sleep
{
    public class Startup : FunctionsStartup
    {
        private static ILogger _logger;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddLogging();
            _logger = new LoggerFactory().CreateLogger(nameof(CreateSleepDocument));

            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
                {
                    MaxRetryAttemptsOnRateLimitedRequests = 3,
                    MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(60)
                };
                return new CosmosClient(configuration["CosmosDBConnectionString"], cosmosClientOptions);
            });

            builder.Services.AddSingleton<IServiceBusHelpers>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                return new ServiceBusHelpers(configuration["ServiceBusConnectionString"]);
            });
            builder.Services.AddTransient<ISleepRepository, SleepRepository>();
            builder.Services.AddScoped<ISleepService, SleepService>();
        }
    }
}
