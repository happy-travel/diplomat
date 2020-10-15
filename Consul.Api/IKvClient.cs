using System.Threading.Tasks;

namespace Diplomat.Consul.Api
{
    public interface IKvClient
    {
        Task<KvPair> Get(string key);

        Task<T> GetValue<T>(string key);
    }
}