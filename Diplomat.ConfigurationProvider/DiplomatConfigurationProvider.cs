using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Diplomat.ConfigurationProvider
{
    // Based on https://www.natmarchand.fr/consul-configuration-aspnet-core/
    public class DiplomatConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider
    {
        public DiplomatConfigurationProvider(List<Uri> consulUrls, string key, string token, int delayOnFailureInSeconds)
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
            Data = await ExecuteQueryAsync();

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
                    Data = await ExecuteQueryAsync(consulUrlIndex, true);
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

      
        private async Task<IDictionary<string, string>> ExecuteQueryAsync(int consulUrlIndex = 0, bool isBlocking = false)
        {
            var requestUri = isBlocking ? $"?index={_consulConfigurationIndex}" : "";
            using var request =
                new HttpRequestMessage(HttpMethod.Get, new Uri(_consulUrls[consulUrlIndex], requestUri));
            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            if (response.Headers.Contains(ConsulIndexHeader))
            {
                var indexValue = response.Headers.GetValues(ConsulIndexHeader).FirstOrDefault();
                int.TryParse(indexValue, out _consulConfigurationIndex);
            }

            var tokens = JToken.Parse(await response.Content.ReadAsStringAsync());
            return tokens
                .Select(k => KeyValuePair.Create
                (
                    key: string.Empty,
                    value: JToken.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(k.Value<string>("Value"))))
                ))
                .SelectMany(Flatten)
                .ToDictionary(v => ConfigurationPath.Combine(v.Key.Split('/')), v => v.Value,
                    StringComparer.OrdinalIgnoreCase);
        }


        private static IEnumerable<KeyValuePair<string, string>> Flatten(KeyValuePair<string, JToken> tuple)
        {
            if (!(tuple.Value is JObject value))
                yield break;

            foreach (var property in value)
            {
                var propertyKey = !string.IsNullOrEmpty(tuple.Key)
                    ? $"{tuple.Key}/{property.Key}"
                    : property.Key;

                switch (property.Value.Type)
                {
                    case JTokenType.Object:
                        foreach (var item in Flatten(KeyValuePair.Create(propertyKey, property.Value)))
                            yield return item;

                        break;
                    case JTokenType.Array:
                        break;
                    default:
                        yield return KeyValuePair.Create(propertyKey, property.Value.Value<string>());

                        break;
                }
            }
        }


        private const string ConsulIndexHeader = "X-Consul-Index";

        private readonly HttpClient _httpClient;
        private readonly IReadOnlyList<Uri> _consulUrls;
        private readonly int _delayOnFailureInSeconds;
        private readonly Task _configurationListeningTask;
        private int _consulConfigurationIndex;
    }
}