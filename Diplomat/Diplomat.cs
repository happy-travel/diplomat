using System.Threading.Tasks;
using HappyTravel.Diplomat.Abstractions;

namespace HappyTravel.Diplomat
{
    public class Diplomat
    {
        internal Diplomat(ISettingsProvider provider)
        {
            _provider = provider;
        }


        public ValueTask<byte[]> Get(string key) 
            => _provider.Get(key);


        public ValueTask<T> Get<T>(string key) 
            => _provider.Get<T>(key);


        private readonly ISettingsProvider _provider;
    }
}
