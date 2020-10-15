using System;
using Diplomat.Consul.Api.Constants;

namespace Diplomat.Consul.Api
{
    public class ConfigFactory
    {
        public static Config FromEnvironment() 
            => BuildDefault();


        private static Config BuildDefault()
        {
            var result = new Config();

            var address = FromEnv(ConfigConstants.HttpAddrEnvName);
            if (!string.IsNullOrWhiteSpace(address))
                result.Address = address;

            bool.TryParse(FromEnv(ConfigConstants.HttpSslEnvName), out var isSsl);
            if (isSsl)
                result.Scheme = "https";

            var token = FromEnv(ConfigConstants.HttpTokenEnvName);
            if (!string.IsNullOrWhiteSpace(token))
                result.Token = token;

            return result;


            static string? FromEnv(string key) 
                => Environment.GetEnvironmentVariable(key);
        }
    }
}
