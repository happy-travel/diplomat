using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace HappyTravel.ConsulKeyValueClient.ConfigurationProvider.Extensions
{
    // Based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddConsulKeyValueClient(this IConfigurationBuilder configurationBuilder, 
            List<Uri> urls, string key, string token, int delayOnFailureInSeconds = 60)
        {
            return configurationBuilder.Add(new ConsulKeyValueClientConfigurationSource(urls, key, token, delayOnFailureInSeconds));
        }
        
        
        public static IConfigurationBuilder AddConsulKeyValueClient(this IConfigurationBuilder configurationBuilder, 
            string url, string key, string token, int delayOnFailureInSeconds = 60)
        {
            return configurationBuilder.Add(new ConsulKeyValueClientConfigurationSource(new List<Uri>{ new(url) }, key, token, delayOnFailureInSeconds));
        }
    }
}