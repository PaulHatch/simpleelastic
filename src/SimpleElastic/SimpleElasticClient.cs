using SimpleElastic.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleElastic
{
    /// <summary>
    /// A light-weight client for connecting to an elasticsearch cluster.
    /// </summary>
    public sealed class SimpleElasticClient
    {
        private const string _mediaType = "application/json";
        private static readonly JsonSerializerSettings _defaultJsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private static readonly Lazy<HttpClient> _defaultClient = new Lazy<HttpClient>(CreateClient);
        private static HttpClient CreateClient()
        {
            return new HttpClient();
        }

        private readonly HttpMessageInvoker _client;
        private readonly ILogger _log;
        private readonly JsonSerializerSettings _jsonSettings;
        private readonly IHostProvider _hostProvider;

        public SimpleElasticClient(ClientOptions config)
        {
            _client = config.HttpClient ?? _defaultClient.Value;
            _log = config.Logger ?? NullLogger.Instance;
            _jsonSettings = config.SerializerSettings ?? _defaultJsonSettings;
            _hostProvider = config.HostProvider ?? throw new ArgumentNullException($"{nameof(ClientOptions.HostProvider)} cannot be null", nameof(config));
        }

        /// <summary>
        /// Executes a _search with the specified parameters.
        /// </summary>
        /// <typeparam name="TSource">
        /// The index type model to use, this should support the mapping from the '_source'
        /// document object.
        /// </typeparam>
        /// <param name="index">The index (or indexes or index pattern) to search.</param>
        /// <param name="query">The query object.</param>
        /// <param name="options">The options for the search query.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns>The result of the requested search.</returns>
        public async Task<SearchResult<TSource>> SearchAsync<TSource>(string index, object query, object options = null, CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + $"{index}/_search{QueryStringParser.GetQueryString(options)}");
            var result = await MakeRequest<SearchResponse<TSource>>(HttpMethod.Post, requestUri, query, cancel);

            return new SearchResult<TSource>(
                hits: result.Hits.Hits.Select(h =>
                {
                    if (h.Source is IScoreDocument scoreDoc)
                        scoreDoc.Score = h.Score;

                    return h.Source;
                }),
                total: result.Hits.Total,
                aggregations: result.Aggregations,
                suggestions: null
            );
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
                    throw new SimpleElasticHttpException($"Request returned {response.StatusCode} status code");
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
