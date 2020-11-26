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
    public class Client
    {
        public Client(IHttpClientFactory httpClientFactory, IOptions<Config> config)
        {
            Config = config.Value;
            HttpClientFactory = httpClientFactory;

            _jsonSerializer = new JsonSerializer();
        }


        protected ValueTask<bool> Delete(string path, QueryOptions options)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, path);
            return Send<bool>(request);
        }


        protected async ValueTask<List<T>> Get<T>(string path, QueryOptions options)
        {
            var pathWithQuery = AppendQueryOptions(path, options);
            var request = new HttpRequestMessage(HttpMethod.Get, pathWithQuery);
            var result = await Send<List<T>?>(request);

            return result ?? Enumerable.Empty<T>().ToList();
        }


        protected ValueTask<bool> Put(string path, Stream payload, QueryOptions options)
        {
            payload.Seek(0, SeekOrigin.Begin);
            var request = new HttpRequestMessage(HttpMethod.Put, path)
            {
                Content = new StreamContent(payload, (int) payload.Length)
            };

            return Send<bool>(request, HttpUploadClientName);
        }


        private static string AppendQueryOptions(string path, QueryOptions options)
        {
            var kvs = new Dictionary<string, object>();
            if (options.IsRecursive)
                kvs.Add("recurse", true);

            if (!kvs.Any())
                return path;

            var builder = new StringBuilder();
            foreach (var (key, value) in kvs)
                builder.AppendFormat("&{0}={1}", key, value);

            builder[0] = '?';
            builder.Insert(0, path);

            return builder.ToString();
        }


        private async ValueTask<T> Send<T>(HttpRequestMessage request, string clientName = HttpClientName)
        {
            using var client = HttpClientFactory.CreateClient(clientName);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
            
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);

            return _jsonSerializer.Deserialize<T>(jsonReader)!;
        }


        internal const string HttpClientName = "ConsulHttpClient";
        internal const string HttpUploadClientName = "ConsulUploadHttpClient";

        protected readonly Config Config;
        protected readonly IHttpClientFactory HttpClientFactory;
        private readonly JsonSerializer _jsonSerializer;
    }
}
