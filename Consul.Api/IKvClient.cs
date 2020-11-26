using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diplomat.Consul.Api
{
    public interface IKvClient
    {
        ValueTask<bool> Create<T>(string key, T value, QueryOptions? options = null);

        ValueTask<bool> Delete(string key, QueryOptions? options = null);

        ValueTask<List<KvPair>> Get(string key, QueryOptions? options = null);

        ValueTask<T> GetValue<T>(string key, QueryOptions? options = null);

        ValueTask<Dictionary<string, T>> GetValues<T>(string key, QueryOptions? options = null);

        ValueTask<bool> Update<T>(string key, T value, QueryOptions? options = null);
    }
}