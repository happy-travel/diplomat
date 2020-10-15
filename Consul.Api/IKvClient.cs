using System.Threading.Tasks;

namespace Diplomat.Consul.Api
{
    public interface IKvClient
    {
        Task<bool> Delete(string key);

        Task<KvPair> Get(string key);

        Task<T> GetValue<T>(string key);
    }
}