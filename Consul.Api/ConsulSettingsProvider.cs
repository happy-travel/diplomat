using System.Threading.Tasks;
using Diplomat.Abstractions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Diplomat.Consul.Api
{
    public class ConsulSettingsProvider : ISettingsProvider
    {
        public ConsulSettingsProvider(IOptions<DiplomatOptions> options, IKvClient kvClient)
        {
            _kvClient = kvClient;

            _options = options.Value;
        }


        public async ValueTask<T> Get<T>(string key) => await _kvClient.GetValue<T>(BuildKey(key));


        public void SetSettings()
        { }


        private string BuildKey(string key) 
            => $"{_options.KeyPrefix}/{key}";


        private readonly DiplomatOptions _options;
        private readonly IKvClient _kvClient;
    }
}
