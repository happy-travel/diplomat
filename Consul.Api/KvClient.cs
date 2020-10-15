using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Diplomat.Consul.Api
{
    public class KvClient : Client, IKvClient
    {
        public KvClient(IHttpClientFactory httpClientFactory, IOptions<Config> config) : base(httpClientFactory, config)
        {
            _jsonSerializer = new JsonSerializer();
        }


        public async Task<KvPair> Get(string key)
        {
            var path = BuildPath(key);
            var pairs =  await Get<KvPair>(path);

            return pairs.FirstOrDefault()!;
        }


        public async Task<T> GetValue<T>(string key)
        {
            var pair = await Get(key);
            if (pair.Value is null)
                return default!;

            await using var stream = new MemoryStream(pair.Value);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(reader);

            return _jsonSerializer.Deserialize<T>(jsonReader);
        }


        private static string BuildPath(string key)
        {
            var sanitizedKey = key.StartsWith('/')
                ? key.Substring(1)
                : key;

            return KvPathSegment + sanitizedKey;
        }


        private const string KvPathSegment = "/v1/kv/";

        private readonly JsonSerializer _jsonSerializer;
    }
}
