using Elasticsearch.LiteClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elasticsearch.LiteClient
{
    public sealed class ElasticClient
    {
        private const string _mediaType = "application/json";
        private readonly JsonSerializerSettings _defaultJsonSettings;
        private static readonly Lazy<HttpClient> _defaultClient = new Lazy<HttpClient>(CreateClient);
        private static HttpClient CreateClient()
        {
            return new HttpClient();
        }

        private readonly HttpMessageInvoker _client;
        private readonly ILogger _log;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly IHostProvider _hostProvider;

        public ElasticClient(ClientOptions config)
        {
            _client = config.HttpClient ?? _defaultClient.Value;
            _log = config.Logger ?? NullLogger.Instance;
            _jsonSettings = config.SerializerSettings ?? _defaultJsonSettings;
            _hostProvider = config.HostProvider ?? throw new ArgumentNullException($"{nameof(ClientOptions.HostProvider)} cannot be null", nameof(config));
        }

        public async Task<SearchResult<T>> SearchAsync<T>(string index, object query, CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = _hostProvider.Next();
            var result = await MakeRequest<SearchResponse<T>>(HttpMethod.Post, requestUri, query, cancel);


            return new SearchResult<T>
            {
            };
        }

        private async Task<T> MakeRequest<T>(HttpMethod method, Uri requestUri, object query, CancellationToken cancel)
        {
            var requestBody = JsonConvert.SerializeObject(query, _jsonSettings);
            using (var content = new StringContent(requestBody, Encoding.UTF8, _mediaType))
            using (var request = new HttpRequestMessage(method, requestUri) { Content = content })
            using (var response = await _client.SendAsync(request, cancel))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new ElasticsearchHttpException($"Request returned {response.StatusCode} status code");
                }

                using (var responseContent = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(responseContent))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var deserializer = new JsonSerializer { ContractResolver = _jsonSettings.ContractResolver };
                    return deserializer.Deserialize<T>(jsonReader);
                }
            }
        }
    }
}
