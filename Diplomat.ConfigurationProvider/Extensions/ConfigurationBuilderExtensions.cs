using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace HappyTravel.Diplomat.ConfigurationProvider.Extensions
{
    // Based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDiplomat(this IConfigurationBuilder configurationBuilder, 
            List<Uri> urls, string key, string token, int delayOnFailureInSeconds = 60)
        {
            return configurationBuilder.Add(new DiplomatConfigurationSource(urls, key, token, delayOnFailureInSeconds));
        }
        
        
        public static IConfigurationBuilder AddDiplomat(this IConfigurationBuilder configurationBuilder, 
            string url, string key, string token, int delayOnFailureInSeconds = 60)
        {
            return configurationBuilder.Add(new DiplomatConfigurationSource(new List<Uri>{ new(url) }, key, token, delayOnFailureInSeconds));
        }
    }
}