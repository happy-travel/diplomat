using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HappyTravel.Diplomat.Extensions;
using HappyTravel.Diplomat.Abstractions;
using HappyTravel.Diplomat.ConfigurationProvider.Extensions;
using HappyTravel.Diplomat.Consul.Api;
using HappyTravel.Diplomat.Consul.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.Diplomat.IntegrationConsole
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