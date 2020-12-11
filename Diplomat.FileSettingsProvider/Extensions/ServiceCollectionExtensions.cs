using HappyTravel.Diplomat.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HappyTravel.Diplomat.FileSettingsProvider.Extensions
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
