using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diplomat.Abstractions;

namespace Diplomat.FileSettingsProvider
{
    public class FileSettingsProvider : ISettingsProvider
    {
        public FileSettingsProvider(ISettingsReader settingsReader)
        {
            _settingsReader = settingsReader;
        }


        public ValueTask<bool> Create<T>(string key, T value)
        {
            if (value is null)
                return new ValueTask<bool>(false);

            var result = _settings.TryAdd(key, value);

            return new ValueTask<bool>(result);
        }


        public ValueTask<bool> Delete(string key)
        {
            _settings.Remove(key);

            return new ValueTask<bool>(true);
        }


        public ValueTask<T> Get<T>(string key) 
            => _settings.TryGetValue(key, out var value) 
                ? new ValueTask<T>((T) value) 
                : default;


        public ValueTask<Dictionary<string, T>> GetValues<T>(string key)
        {
            var results = _settings.Keys
                .Where(sk => sk.StartsWith(key))
                .ToDictionary(sk => sk, sk => (T) _settings[sk]);

            return new ValueTask<Dictionary<string, T>>(results!);
        }


        public ValueTask<bool> Update<T>(string key, T value)
        {
            if (value is null)
                return new ValueTask<bool>(false);

            _settings[key] = value;

            return new ValueTask<bool>(true);
        }


        public void SetSettings() 
            => _settings = _settingsReader.Read();


        private static Dictionary<string, object> _settings = new Dictionary<string, object>();
        
        private readonly ISettingsReader _settingsReader;
    }
}
