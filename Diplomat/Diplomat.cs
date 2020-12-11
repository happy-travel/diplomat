using System.Threading.Tasks;
using HappyTravel.Diplomat.Abstractions;

namespace Diplomat
{
    public class Diplomat
    {
        internal Diplomat(ISettingsProvider provider)
        {
            _provider = provider;
        }


        public async ValueTask<T> Get<T>(string key) 
            => await _provider.Get<T>(key);


        private readonly ISettingsProvider _provider;
    }
}
