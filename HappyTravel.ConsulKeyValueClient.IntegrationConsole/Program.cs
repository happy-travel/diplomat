using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HappyTravel.ConsulKeyValueClient.Extensions;
using HappyTravel.ConsulKeyValueClient.Abstractions;
using HappyTravel.ConsulKeyValueClient.ConfigurationProvider.Extensions;
using HappyTravel.ConsulKeyValueClient.ConsulClient;
using HappyTravel.ConsulKeyValueClient.ConsulClient.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.ConsulKeyValueClient.IntegrationConsole
{
    internal class Program
    {
        private static async Task Main(string[] _)
        {
            var consulUrl = Environment.GetEnvironmentVariable("CONSUL_HTTP_ADDR");
            var consulPath = Environment.GetEnvironmentVariable("CONSUL_PATH");
            var consulToken = Environment.GetEnvironmentVariable("CONSUL_HTTP_TOKEN");
            
            // Testing Diplomat as a service
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<DiplomatOptions>(o =>
                    {
                        o.LocalSettingsPath = @"..\..\..\..\test-settings.json";
                        o.KeyPrefix = consulPath;
                    });
                    services.AddDiplomat();
                    services.AddConsulDiplomatProvider(ConfigFactory.FromEnvironment());
                }).UseConsoleLifetime();

            var host = builder.Build();

            // Testing Diplomat as a configuration provider
            var config = new ConfigurationBuilder()
                .AddDiplomat(new List<Uri> {new(consulUrl)}, consulPath, consulToken)
                .Build();

            var debug = config.GetDebugView();
            host.Run();
        }
    }
}