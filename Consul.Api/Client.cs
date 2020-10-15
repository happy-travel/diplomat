using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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


        protected async Task<List<T>> Get<T>(string path)
        {
            using var client = HttpClientFactory.CreateClient(HttpClientName);
            using var response = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();
            
            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);

            return _jsonSerializer.Deserialize<List<T>>(jsonReader) ?? Enumerable.Empty<T>().ToList();
        }



        internal const string HttpClientName = "ConsulHttpClient";

        protected readonly Config Config;
        protected readonly IHttpClientFactory HttpClientFactory;
        private readonly JsonSerializer _jsonSerializer;
    }
}
