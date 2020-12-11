using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Diplomat;
using Diplomat.Extensions;
using HappyTravel.Diplomat.Abstractions;
using HappyTravel.Diplomat.Consul.Api;
using HappyTravel.Diplomat.Consul.Api.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationConsole
{
    internal class Program
    {
        private static async Task Main(string[] _)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<DiplomatOptions>(o =>
                    {
                        o.LocalSettingsPath = @"..\..\..\..\test-settings.json";
                        o.KeyPrefix = "tsutsujigasaki/development";
                    });
                    services.AddDiplomat();
                    services.AddConsulDiplomatProvider(ConfigFactory.FromEnvironment());
                }).UseConsoleLifetime();
 
            var host = builder.Build();

            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var factory = serviceProvider.GetRequiredService<IDiplomatFactory>();
            var diplomat = factory.Create();
            var result = await diplomat.Get("");

            var config = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(result))
                .Build();

            Console.WriteLine(result);
        }
    }
}
