using HappyTravel.ConsulKeyValueClient.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.ConsulKeyValueClient.FileSettingsProvider.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileDiplomatProvider(this IServiceCollection services)
        {
            services.AddTransient<ISettingsReader, FileSettingsReader>();
            services.AddTransient<ISettingsProvider, FileSettingsProvider>();

            return services;
        }
    }
}
