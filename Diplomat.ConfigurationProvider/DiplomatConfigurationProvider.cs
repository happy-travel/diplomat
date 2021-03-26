using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;

namespace HappyTravel.Diplomat.ConfigurationProvider
{
    // Based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public class DiplomatConfigurationProvider : JsonStreamConfigurationProvider
    {
        public DiplomatConfigurationProvider(
            JsonStreamConfigurationSource source,
            List<Uri> consulUrls,
            string key,
            string token,
            int delayOnFailureInSeconds
        ) : base(source)
        {
            _consulUrls = consulUrls
                .Select(url => new Uri(url, $"v1/kv/{key}"))
                .ToList();

            if (_consulUrls.Count <= 0)
                throw new ArgumentOutOfRangeException(nameof(consulUrls));

            _httpClient = CreateHttpClient(token);
            _delayOnFailureInSeconds = delayOnFailureInSeconds;
            _configurationListeningTask = new Task(ListenToConfigurationChanges);
        }
        
        
        private static HttpClient CreateHttpClient(string token)
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };
            var client = new HttpClient(handler, true);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
        
        
        public override void Load()
        {
            LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }


        private async Task LoadAsync()
        {
            base.Load(await ExecuteQueryAsync());

            if (_configurationListeningTask.Status == TaskStatus.Created)
                _configurationListeningTask.Start();
        }
        
        
        private async void ListenToConfigurationChanges()
        {
            var consulUrlIndex = 0;
            while (true)
            {
                try
                {
                    base.Load(await ExecuteQueryAsync(consulUrlIndex, true));
                    OnReload();
                }
                catch
                {
                    consulUrlIndex = GetNextConsulUrlIndex(consulUrlIndex);
                    if (consulUrlIndex == 0)
                        await Task.Delay(TimeSpan.FromSeconds(_delayOnFailureInSeconds));
                }
            }
        }
        
        
        private int GetNextConsulUrlIndex(int current)
        {
            var next = current + 1;
            return next < _consulUrls.Count
                ? next
                : 0;
        }
       
        
        private async Task<Stream> ExecuteQueryAsync(int consulUrlIndex = 0, bool isBlocking = false)
        {
            var requestUri = isBlocking ? $"?index={_consulConfigurationIndex}" : string.Empty;
            using var request =
                new HttpRequestMessage(HttpMethod.Get, new Uri(_consulUrls[consulUrlIndex], requestUri));
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.Headers.Contains(ConsulIndexHeader))
            {
                var indexValue = response.Headers.GetValues(ConsulIndexHeader).FirstOrDefault();
                int.TryParse(indexValue, out _consulConfigurationIndex);
            }

            await using var stream =  await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);
            var data = document.RootElement
                .EnumerateArray()
                .Single() // The call to Consul is not recursive. The response will always be a JSON array with a single element
                .GetProperty("Value")
                .GetString();

            return new MemoryStream(Convert.FromBase64String(data));
        }


        private const string ConsulIndexHeader = "X-Consul-Index";

        private readonly HttpClient _httpClient;
        private readonly IReadOnlyList<Uri> _consulUrls;
        private readonly int _delayOnFailureInSeconds;
        private readonly Task _configurationListeningTask;
        private int _consulConfigurationIndex;
    }
}