using Microsoft.Extensions.Configuration;

namespace Diplomat.ConfigurationProvider.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDiplomat(this IConfigurationBuilder configurationBuilder, 
            string address, string key, string token = null)
        {
            return configurationBuilder.Add(new DiplomatConfigurationSource(address, key, token));
        }
    }
}