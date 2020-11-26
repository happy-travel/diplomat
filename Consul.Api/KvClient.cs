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


        public ValueTask<bool> Create<T>(string key, T value, QueryOptions? options = null) 
            => Put(key, value);


        public new ValueTask<bool> Delete(string key, QueryOptions? options = null)
        {
            options ??= QueryOptions.Default;

            var path = BuildPath(key);
            return base.Delete(path, options);
        }


        public async ValueTask<List<KvPair>> Get(string key, QueryOptions? options = null)
        {
            options ??= QueryOptions.Default;

            var path = BuildPath(key);
            return await Get<KvPair>(path, options);
        }


        public async ValueTask<T> GetValue<T>(string key, QueryOptions? options = null)
        {
            var values = await GetValues<T>(key, options);
            if (!values.Any())
                return default!;

            if (values.TryGetValue(key, out var value))
                return value;

            return default!;
        }


        public async ValueTask<Dictionary<string, T>> GetValues<T>(string key, QueryOptions? options = null)
        {
            options ??= QueryOptions.Default;
            options.IsRecursive = true;

            var pairs = await Get(key, options);
            var results = new Dictionary<string, T>();
            foreach (var pair in pairs)
            {
                if (pair.Value is null)
                {
                    results.Add(pair.Key, default!);
                    continue;
                }

                await using var stream = new MemoryStream(pair.Value);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var jsonReader = new JsonTextReader(reader);

                var value = _jsonSerializer.Deserialize<T>(jsonReader);
                results.Add(pair.Key, value!);
            }

            return results;
        }


        public ValueTask<bool> Update<T>(string key, T value, QueryOptions? options = null) 
            => Put(key, value);


        private static string BuildPath(string key)
        {
            var sanitizedKey = key.StartsWith('/')
                ? key.Substring(1)
                : key;

            return KvPathSegment + sanitizedKey;
        }


        private async ValueTask<bool> Put<T>(string key, T value, QueryOptions? options = null)
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
