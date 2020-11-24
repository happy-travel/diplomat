using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diplomat.Abstractions
{
    public interface ISettingsProvider
    {
        ValueTask<bool> Create<T>(string key, T value);

        ValueTask<bool> Delete(string key);

        ValueTask<T> Get<T>(string key);

        ValueTask<Dictionary<string, T>> GetValues<T>(string key);

        ValueTask<bool> Update<T>(string key, T value);

        void SetSettings();
    }
}