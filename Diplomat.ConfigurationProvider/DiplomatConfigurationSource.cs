using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Diplomat.ConfigurationProvider
{
    // Based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public class DiplomatConfigurationSource : IConfigurationSource
    {
        public DiplomatConfigurationSource(List<Uri> urls, string key, string token, int delayOnFailureInSeconds)
        {
            _urls = urls;
            _key = key;
            _token = token;
            _delayOnFailureInSeconds = delayOnFailureInSeconds;
        }
        
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DiplomatConfigurationProvider(_urls, _key, _token, _delayOnFailureInSeconds);
        }


        private readonly List<Uri> _urls;
        private readonly string _key;
        private readonly string _token;
        private readonly int _delayOnFailureInSeconds;
    }
}