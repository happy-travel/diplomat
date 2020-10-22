using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Diplomat.Consul.Api;
using Diplomat.Consul.Api.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationConsole
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddConsul(ConfigFactory.FromEnvironment());
                }).UseConsoleLifetime();
 
            var host = builder.Build();

            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var kvClient = serviceProvider.GetRequiredService<IKvClient>();
            var results = await kvClient.Get("1", new QueryOptions { IsRecursive = true});

            Console.WriteLine(results);
        }
    }
}
