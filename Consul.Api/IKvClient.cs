using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diplomat.Consul.Api
{
    public interface IKvClient
    {
        Task<bool> Create<T>(string key, T value, QueryOptions? options = null);

        Task<bool> Delete(string key, QueryOptions? options = null);

        Task<List<KvPair>> Get(string key, QueryOptions? options = null);

        Task<T> GetValue<T>(string key, QueryOptions? options = null);

        Task<bool> Update<T>(string key, T value, QueryOptions? options = null);
    }
}