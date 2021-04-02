using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.ConsulKeyValueClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDiplomat(this IServiceCollection services)
        {
            services.AddSingleton<IDiplomatFactory, DiplomatFactory>();
            services.AddTransient<Diplomat>();

            return services;
        }
    }
}
