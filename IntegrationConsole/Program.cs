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
                    services.AddConsul(new Config
                    {
                        Address = "consul-dev.happytravel.com",
                        Scheme = "https",
                        Token = "e2845507-3111-8bb7-c1e0-a5bb7da5b8a5"
                    });
                }).UseConsoleLifetime();
 
            var host = builder.Build();

            using var serviceScope = host.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var kvClient = serviceProvider.GetRequiredService<IKvClient>();
            var result = await kvClient.Create("1/test-1", "the new test value from the client 123");

            Console.WriteLine(result);
        }
    }
}
