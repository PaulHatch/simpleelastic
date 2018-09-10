using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleElastic.Models;
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
        /// Initializes a new instance of the <see cref="SimpleElasticClient"/> class with
        /// a single specified host and default options.
        /// </summary>
        /// <param name="host">The elasticsearch host to connect to.</param>
        public SimpleElasticClient(Uri host)
            : this(new ClientOptions(host))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticClient"/> class with
        /// a single specified host and default options
        /// </summary>
        /// <param name="host">The elasticsearch host to connect to.</param>
        public SimpleElasticClient(string host)
            : this(new ClientOptions(new Uri(host)))
        {
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

            var result = await MakeRequestAsync<SearchResponse<TSource>>(
                HttpMethod.Post,
                requestUri,
                JsonConvert.SerializeObject(query, _jsonSettings),
                MediaTypes.ApplicationJson,
                cancel);

            return new SearchResult<TSource>(
                hits: result.Hits.Hits.Select(h =>
                {
                    if (h.Source is IScoreDocument scoreDoc)
                    {
                        scoreDoc.Score = h.Score;
                    }

                    return h.Source;
                }),
                total: result.Hits.Total,
                aggregations: result.Aggregations,
                suggestions: null
            );
        }

        /// <summary>
        /// Execute bulk index, create, update, or delete actions. This is the simpler version which
        /// assumes you are only performing a single action type on a single index and document type.
        /// </summary>
        /// <param name="index">The index and type.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="documents">
        /// The documents, if <see cref="BulkActionType.Index"/> or <see cref="BulkActionType.Create"/>
        /// is specified, this is the source document to index or create, if <see cref="BulkActionType.Update"/>
        /// is specified this should be an update statement, and if <see cref="BulkActionType.Delete"/> is specified
        /// this is either the ID or a document which implements <see cref="IKeyDocument"/>.
        /// </param>
        /// <param name="options">Any options for the request such as 'refresh'.</param>
        /// <param name="throwOnFailure">
        /// If true, an exception will be thrown if any of the requested actions fail. (Normally the _bulk
        /// API endpoint returns a 200 if the request was processed successfully, even if document actions fail.)
        /// </param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">An index is required - index</exception>
        public async Task<BulkActionResult> BulkActionAsync(string index, BulkActionType actionType, IEnumerable<object> documents, object options = null, bool throwOnFailure = true, CancellationToken cancel = default(CancellationToken))
        {
            if (index == null)
            {
                throw new ArgumentNullException("An index is required", nameof(index));
            }

            // No punishment for providing null/empty requests
            if (documents?.Any() != true)
            {
                return new BulkActionResult
                {
                    Took = TimeSpan.Zero,
                    HasErrors = false,
                    Items = Array.Empty<BulkActionResultItem>()
                };
            }

            var actionName = actionType.ToString().ToLower();
            var request = new StringBuilder();

            foreach (var document in documents)
            {
                if (actionType == BulkActionType.Delete)
                {
                    var key = (document as IKeyDocument)?.Key ?? document ?? throw new ArgumentException("No key was provided for delete operation", nameof(documents));
                    var header = JsonConvert.SerializeObject(new BulkActionRequest(actionType) { ID = key }, _jsonSettings);
                    request.Append($"{header}\n");
                }
                else
                {
                    var header = JsonConvert.SerializeObject(new BulkActionRequest(actionType) { }, _jsonSettings);
                    var body = JsonConvert.SerializeObject(document, _jsonSettings);
                    request.Append($"{header}\n{body}\n");
                }
            }

            var requestUri = new Uri(_hostProvider.Next() + $"{index}/_bulk{QueryStringParser.GetQueryString(options)}");
            var response = await MakeRequestAsync<BulkActionResult>(
                HttpMethod.Post,
                requestUri,
                request.ToString(),
                MediaTypes.ApplicationNewlineDelimittedJson,
                cancel);

            return response;
        }

        /// <summary>
        /// Execute bulk index, create, update, or delete actions. This version uses explicit action parameters
        /// for each action to be executed.
        /// </summary>
        /// <param name="index">The index and type.</param>
        /// <param name="requests">
        /// The documents, if <see cref="BulkActionType.Index"/> or <see cref="BulkActionType.Create"/>
        /// is specified, this is the source document to index or create, if <see cref="BulkActionType.Update"/>
        /// is specified this should be an update statement, and if <see cref="BulkActionType.Delete"/> is specified
        /// this is either the ID or a document which implements <see cref="IKeyDocument"/>.
        /// </param>
        /// <param name="options">Any options for the request such as 'refresh'.</param>
        /// <param name="throwOnFailure">
        /// If true, an exception will be thrown if any of the requested actions fail. (Normally the _bulk
        /// API endpoint returns a 200 if the request was processed successfully, even if document actions fail.)
        /// </param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">An index is required - index</exception>
        public async Task<BulkActionResult> BulkActionAsync(string index, IEnumerable<BulkActionRequest> requests, object options = null, bool throwOnFailure = true, CancellationToken cancel = default(CancellationToken))
        {
            // No punishment for providing null/empty requests
            if (requests?.Any() != true)
            {
                return new BulkActionResult
                {
                    Took = TimeSpan.Zero,
                    HasErrors = false,
                    Items = Array.Empty<BulkActionResultItem>()
                };
            }

            var request = new StringBuilder();

            foreach (var requestItem in requests)
            {
                if (requestItem.Action == BulkActionType.Delete)
                {
                    var header = JsonConvert.SerializeObject(requestItem, _jsonSettings);
                    request.Append($"{header}\n");
                }
                else
                {
                    var header = JsonConvert.SerializeObject(requestItem, _jsonSettings);
                    var body = JsonConvert.SerializeObject(requestItem.Document, _jsonSettings);
                    request.Append($"{header}\n{body}\n");
                }
            }

            var path = String.IsNullOrEmpty(index) ? "_bulk" : $"{index}/_bulk";
            var requestUri = new Uri(_hostProvider.Next() + $"{path}{QueryStringParser.GetQueryString(options)}");
            var response = await MakeRequestAsync<BulkActionResult>(
                HttpMethod.Post,
                requestUri,
                request.ToString(),
                MediaTypes.ApplicationNewlineDelimittedJson,
                cancel);

            return response;
        }


        private async Task<T> MakeRequestAsync<T>(HttpMethod method, Uri requestUri, string requestContent, string mediaType, CancellationToken cancel)
        {
            using (var content = new StringContent(requestContent, Encoding.UTF8, mediaType))
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
