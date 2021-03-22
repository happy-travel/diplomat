using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Diplomat.ConfigurationProvider
{
    // based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public class DiplomatConfigurationSource : IConfigurationSource
    {
        public DiplomatConfigurationSource(List<Uri> urls, string key, string token)
        {
            Urls = urls;
            Key = key;
            Token = token;
        }
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DiplomatConfigurationProvider(Urls, Key, Token);
        }

        private List<Uri> Urls { get; }
        private string Key { get; }
        private string Token { get; }
    }
}