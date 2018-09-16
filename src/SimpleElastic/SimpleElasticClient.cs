using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleElastic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleElastic
{
    /// <summary>
    /// A light-weight client for connecting to an Elasticsearch cluster.
    /// </summary>
    public class SimpleElasticClient
    {
        public static JsonSerializerSettings DefaultJsonSettings { get; set; } = new JsonSerializerSettings
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticClient" /> class with
        /// the specified configuration.
        /// </summary>
        /// <param name="config">The configuration to use.</param>
        public SimpleElasticClient(ClientOptions config)
        {
            _client = config.HttpClient ?? _defaultClient.Value;
            _log = config.Logger ?? NullLogger.Instance;
            _jsonSettings = DefaultJsonSettings;
            _hostProvider = config.HostProvider ?? throw new ArgumentNullException($"{nameof(ClientOptions.HostProvider)} cannot be null", nameof(config));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticClient"/> class with
        /// a single specified host and default options.
        /// </summary>
        /// <param name="host">The Elasticsearch host to connect to.</param>
        public SimpleElasticClient(Uri host)
            : this(new ClientOptions(host))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticClient"/> class with
        /// a single specified host and default options
        /// </summary>
        /// <param name="host">The Elasticsearch host to connect to.</param>
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

                    if (h.Source is IKeyDocument keyDoc)
                    {
                        keyDoc.Key = h.ID;
                    }

                    return h.Source;
                }),
                total: result.Hits.Total,
                aggregations: result.Aggregations,
                suggestions: null
            );
        }

        /// <summary>
        /// Executes a get document by ID request.
        /// </summary>
        /// <typeparam name="TSource">
        /// The index type model to use, this should support the mapping from the '_source'
        /// document object.
        /// </typeparam>
        /// <param name="index">The index to search.</param>
        /// <param name="document">The document type to return.</param>
        /// <param name="id">The document  ID.</param>
        /// <param name="options">The options for the search query.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns>Result of the get request.</returns>
        public Task<GetResult<TSource>> GetAsync<TSource>(
            string index,
            string document,
            string id,
            object options = null,
            CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + $"{index}/{document}/{id}{QueryStringParser.GetQueryString(options)}");
            return MakeRequestAsync<GetResult<TSource>>(HttpMethod.Get, requestUri, cancel);
        }

        /// <summary>
        /// Executes a get document by ID request.
        /// </summary>
        /// <typeparam name="TSource">
        /// The index type model to use, this should support the mapping from the '_source'
        /// document object.
        /// </typeparam>
        /// <param name="index">The index to search.</param>
        /// <param name="document">The document type to return.</param>
        /// <param name="id">The document  ID.</param>
        /// <param name="options">The options for the search query.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns>Result of the get request.</returns>
        public Task<TSource> GetSourceAsync<TSource>(
            string index,
            string document,
            string id,
            object options = null,
            CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + $"{index}/{document}/{id}/_source{QueryStringParser.GetQueryString(options)}");
            return MakeRequestAsync<TSource>(HttpMethod.Get, requestUri, cancel);
        }

        /// <summary>
        /// Executes a put (create) index request.
        /// </summary>
        /// <param name="indexName">Name of the index to create.</param>
        /// <param name="settings">The index create settings.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns>The acknowledgment result.</returns>
        public Task<AcknowledgeResult> CreateIndexAsync(string indexName, object settings, CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + indexName);
            return MakeRequestAsync<AcknowledgeResult>(
                HttpMethod.Put,
                requestUri,
                JsonConvert.SerializeObject(settings),
                MediaTypes.ApplicationJson,
                cancel);
        }

        /// <summary>
        /// Executes a get index request.
        /// </summary>
        /// <param name="indexName">Name of the index to get.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns></returns>
        public Task<IDictionary<string, IndexResult>> GetIndexAsync(string indexName, CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + indexName);
            return MakeRequestAsync<IDictionary<string, IndexResult>>(
                HttpMethod.Get,
                requestUri,
                cancel);
        }

        /// <summary>
        /// Executes a delete index request.
        /// </summary>
        /// <param name="index">Name of the index to delete.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns>The acknowledgment result.</returns>
        public Task<AcknowledgeResult> DeleteIndexAsync(string index, CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + index);
            return MakeRequestAsync<AcknowledgeResult>(
                HttpMethod.Delete,
                requestUri,
                cancel);
        }

        /// <summary>
        /// Executes a request to check if an index exists.
        /// </summary>
        /// <param name="index">Name of the index to check for.</param>
        /// <param name="cancel">A cancellation token for the request.</param>
        /// <returns>True if the check was successful, false if 404 was returned.</returns>
        public async Task<bool> IndexExistsAsync(string index, CancellationToken cancel = default(CancellationToken))
        {
            var requestUri = new Uri(_hostProvider.Next() + index);
            var result = await MakeRequestAsync(HttpMethod.Head, requestUri, cancel);

            return result == HttpStatusCode.OK;
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
        public Task<BulkActionResult> BulkActionAsync(string index, string type, BulkActionType actionType, IEnumerable<object> documents, object options = null, bool throwOnFailure = true, CancellationToken cancel = default(CancellationToken))
        {
            if (index == null)
            {
                throw new ArgumentNullException("An index is required", nameof(index));
            }

            // No punishment for providing null/empty requests
            if (documents?.Any() != true)
            {
                return Task.FromResult(new BulkActionResult
                {
                    Took = TimeSpan.Zero,
                    HasErrors = false,
                    Items = Array.Empty<BulkActionResultItem>()
                });
            }

            var actionName = actionType.ToString().ToLower();
            var request = new StringBuilder();

            foreach (var document in documents)
            {
                if (actionType == BulkActionType.Delete)
                {
                    var key = (document as IKeyDocument)?.Key ?? document ?? throw new ArgumentException("No key was provided for delete operation", nameof(documents));
                    var requestItem = new BulkActionRequest(actionType)
                    {
                        ID = key
                    };
                    request.Append(JsonConvert.SerializeObject(requestItem, _jsonSettings));
                }
                else
                {
                    var requestItem = new BulkActionRequest(actionType)
                    {
                        ID = (document as IKeyDocument)?.Key,
                        Document = document
                    };
                    request.Append(JsonConvert.SerializeObject(requestItem, _jsonSettings));
                }
            }

            var requestUri = new Uri(_hostProvider.Next() + $"{index}/{type}/_bulk{QueryStringParser.GetQueryString(options)}");
            return MakeRequestAsync<BulkActionResult>(
                HttpMethod.Post,
                requestUri,
                request.ToString(),
                MediaTypes.ApplicationNewlineDelimittedJson,
                cancel);
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
        public Task<BulkActionResult> BulkActionAsync(string index, IEnumerable<BulkActionRequest> requests, object options = null, bool throwOnFailure = true, CancellationToken cancel = default(CancellationToken))
        {
            // No punishment for providing null/empty requests
            if (requests?.Any() != true)
            {
                return Task.FromResult(new BulkActionResult
                {
                    Took = TimeSpan.Zero,
                    HasErrors = false,
                    Items = Array.Empty<BulkActionResultItem>()
                });
            }

            var request = new StringBuilder();

            foreach (var requestItem in requests)
            {
                request.Append(JsonConvert.SerializeObject(requestItem, _jsonSettings));
            }

            var path = String.IsNullOrEmpty(index) ? "_bulk" : $"{index}/_bulk";
            var requestUri = new Uri(_hostProvider.Next() + $"{path}{QueryStringParser.GetQueryString(options)}");
            return MakeRequestAsync<BulkActionResult>(
                HttpMethod.Post,
                requestUri,
                request.ToString(),
                MediaTypes.ApplicationNewlineDelimittedJson,
                cancel);
        }

        /// <summary>
        /// Makes an HTTP request without a body.
        /// </summary>
        private async Task<T> MakeRequestAsync<T>(HttpMethod method, Uri requestUri, CancellationToken cancel)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            {
                return await MakeRequestAsync<T>(request, cancel);
            }
        }

        /// <summary>
        /// Makes an HTTP request without a body that does not expect a response body.
        /// </summary>
        private async Task<HttpStatusCode> MakeRequestAsync(HttpMethod method, Uri requestUri, CancellationToken cancel)
        {
            using (var request = new HttpRequestMessage(method, requestUri))
            using (var response = await _client.SendAsync(request, cancel))
            {
                return response.StatusCode;
            }
        }

        /// <summary>
        /// Makes an HTTP request with a body.
        /// </summary>
        private async Task<T> MakeRequestAsync<T>(HttpMethod method, Uri requestUri, string requestContent, string mediaType, CancellationToken cancel)
        {
            using (var content = new StringContent(requestContent))
            {
                // Specifying the charset as UTF-8 breaks elasticsearch, set it without
                // here explicitly for now.
                // https://github.com/elastic/elasticsearch/issues/28123
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType);
                using (var request = new HttpRequestMessage(method, requestUri) { Content = content })
                {
                    return await MakeRequestAsync<T>(request, cancel);
                }
            }
        }

        /// <summary>
        /// Handles HTTP request messages.
        /// </summary>
        private async Task<T> MakeRequestAsync<T>(HttpRequestMessage request, CancellationToken cancel)
        {
            using (var response = await _client.SendAsync(request, cancel))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ErrorResult>(errorResponse);
                    throw new SimpleElasticHttpException($"Error {response.StatusCode} {response.ReasonPhrase}, {error.Error.Type}: {error.Error.Reason}")
                    {
                        Response = error
                    };
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
