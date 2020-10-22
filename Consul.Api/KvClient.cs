using System.Collections.Generic;
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


        public Task<bool> Create<T>(string key, T value, QueryOptions? options = null) 
            => Put(key, value);


        public new Task<bool> Delete(string key, QueryOptions? options = null)
        {
            options ??= QueryOptions.Default;

            var path = BuildPath(key);
            return base.Delete(path, options);
        }


        public async Task<List<KvPair>> Get(string key, QueryOptions? options = null)
        {
            options ??= QueryOptions.Default;

            var path = BuildPath(key);
            return await Get<KvPair>(path, options);
        }


        public async Task<T> GetValue<T>(string key, QueryOptions? options = null)
        {
            var pairs = await Get(key, options);
            if (!pairs.Any())
                return default!;

            var pair = pairs.First();
            if (pair?.Value is null)
                return default!;

            await using var stream = new MemoryStream(pair.Value);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(reader);

            return _jsonSerializer.Deserialize<T>(jsonReader);
        }


        public Task<bool> Update<T>(string key, T value, QueryOptions? options = null) 
            => Put(key, value);


        private static string BuildPath(string key)
        {
            var sanitizedKey = key.StartsWith('/')
                ? key.Substring(1)
                : key;

            return KvPathSegment + sanitizedKey;
        }


        private async Task<bool> Put<T>(string key, T value, QueryOptions? options = null)
        {
            options ??= QueryOptions.Default;

            await using var stream = new MemoryStream();
            await using var writer = new StreamWriter(stream, Encoding.UTF8);
            using var jsonWriter = new JsonTextWriter(writer);

            _jsonSerializer.Serialize(writer, value);
            await jsonWriter.FlushAsync();

            var path = BuildPath(key);

            return await base.Put(path, stream, options);
        }


        private const string KvPathSegment = "/v1/kv/";

        private readonly JsonSerializer _jsonSerializer;
    }
}
