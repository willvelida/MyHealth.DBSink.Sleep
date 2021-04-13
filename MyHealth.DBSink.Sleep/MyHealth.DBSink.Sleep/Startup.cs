﻿using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHealth.Common;
using MyHealth.DBSink.Sleep;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyHealth.DBSink.Sleep
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                return new CosmosClient(configuration["CosmosDBConnectionString"]);
            });

            builder.Services.AddSingleton(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                return new ServiceBusHelpers(configuration["ServiceBusConnectionString"]);
            });
        }
    }
}
