using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HappyTravel.ConsulKeyValueClient.Abstractions;
using Microsoft.Extensions.Options;

namespace HappyTravel.ConsulKeyValueClient.ConsulClient
{
    public class ConsulSettingsProvider : ISettingsProvider
    {
        public ConsulSettingsProvider(IOptions<DiplomatOptions> options, IKvClient kvClient)
        {
            _kvClient = kvClient;

            _options = options.Value;
        }


        public ValueTask<bool> Create<T>(string key, T value, string? keyPrefix = null) 
            => _kvClient.Create(BuildKey(key, keyPrefix), value);


        public ValueTask<bool> Delete(string key, string? keyPrefix = null) 
            => _kvClient.Delete(BuildKey(key, keyPrefix));


        public async ValueTask<byte[]> Get(string key, string? keyPrefix = null)
        {
            var kvPairs = await _kvClient.Get(BuildKey(key, keyPrefix));
            var pair = kvPairs.First();

            return pair.Value!;
        }


        public async ValueTask<T> Get<T>(string key, string? keyPrefix = null) 
            => await _kvClient.GetValue<T>(BuildKey(key, keyPrefix));


        public ValueTask<Dictionary<string, T>> GetValues<T>(string key, string? keyPrefix = null) 
            => _kvClient.GetValues<T>(BuildKey(key, keyPrefix));


        public ValueTask<bool> Update<T>(string key, T value, string? keyPrefix = null) 
            => _kvClient.Update(BuildKey(key, keyPrefix), value);


        public void SetSettings()
        { }


        private string BuildKey(string key, string? prefix = null)
        {
            var pref = prefix ?? _options.KeyPrefix;
            return string.IsNullOrWhiteSpace(key) 
                ? pref 
                : $"{pref}/{key}";
        }


        private readonly DiplomatOptions _options;
        private readonly IKvClient _kvClient;
    }
}
