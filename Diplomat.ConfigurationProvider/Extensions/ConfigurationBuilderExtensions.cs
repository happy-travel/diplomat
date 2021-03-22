using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Diplomat.ConfigurationProvider.Extensions
{
    // based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDiplomat(this IConfigurationBuilder configurationBuilder, 
            List<Uri> urls, string key, string token)
        {
            return configurationBuilder.Add(new DiplomatConfigurationSource(urls, key, token));
        }
    }
}