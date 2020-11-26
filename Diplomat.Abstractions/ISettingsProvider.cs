using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diplomat.Abstractions
{
    public interface ISettingsProvider
    {
        ValueTask<bool> Create<T>(string key, T value, string? keyPrefix = null);

        ValueTask<bool> Delete(string key, string? keyPrefix = null);

        ValueTask<T> Get<T>(string key, string? keyPrefix = null);

        ValueTask<Dictionary<string, T>> GetValues<T>(string key, string? keyPrefix = null);

        ValueTask<bool> Update<T>(string key, T value, string? keyPrefix = null);

        void SetSettings();
    }
}