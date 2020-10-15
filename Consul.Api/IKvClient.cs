using System.Threading.Tasks;

namespace Diplomat.Consul.Api
{
    public interface IKvClient
    {
        Task<bool> Create<T>(string key, T value);

        Task<bool> Delete(string key);

        Task<KvPair> Get(string key);

        Task<T> GetValue<T>(string key);

        Task<bool> Update<T>(string key, T value);
    }
}