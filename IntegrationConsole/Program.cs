using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Diplomat;
using Diplomat.Extensions;
using HappyTravel.Diplomat.Abstractions;
using HappyTravel.Diplomat.Consul.Api;
using HappyTravel.Diplomat.Consul.Api.Extensions;
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
                        o.KeyPrefix = "nagoya/development";
                    });
                    services.AddDiplomat();
                    services.AddConsulDiplomatProvider(ConfigFactory.FromEnvironment());
                }).UseConsoleLifetime();
 
            var host = builder.Build();

            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var factory = serviceProvider.GetRequiredService<IDiplomatFactory>();
            var diplomat = factory.Create();
            var result = await diplomat.Get<string>("test-keys/1");

            Console.WriteLine(result);
        }
    }
}
