using Microsoft.Extensions.Configuration;

namespace Diplomat.ConfigurationProvider
{
    public class DiplomatConfigurationSource : IConfigurationSource
    {
        public DiplomatConfigurationSource(string address, string key, string token)
        {
            Address = address;
            Key = key;
            Token = token;
        }
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DiplomatConfigurationProvider(Address, Key, Token);
        }

        private string Address { get; }
        private string Key { get; }
        private string Token { get; }
    }
}