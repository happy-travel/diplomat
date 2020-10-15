using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace Diplomat.Consul.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, Config config)
        {
            services.Configure<Config>(c => c = config);

            services.AddHttpClient(Client.HttpClientName, c =>
            {
                c.BaseAddress = new Uri($"{config.Scheme}://{config.Address}");
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.Token);
            });

            services.AddTransient<IKvClient, KvClient>();

            return services;
        }
    }
}
