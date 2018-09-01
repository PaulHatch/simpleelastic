using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Elasticsearch.LiteClient
{
    public class ClientOptions
    {
        /// <summary>
        /// Gets or sets the host provider.
        /// </summary>
        public IHostProvider HostProvider { get; set; }

        /// <summary>
        /// Gets or sets the HTTP client to use when making connections.
        /// If none is provided a default HttpClient instance will be used.
        /// This client should be reused, creating a new instance on each
        /// request will result in degraded performance. <see cref=""/>
        /// </summary>
        public HttpMessageInvoker HttpClient { get; set; }

        /// <summary>
        /// Gets or sets the serializer settings to use when handling requests,
        /// if none is provided, the default settings will be used which include
        /// camel case names contract resolved (i.e. FieldName -> fieldName).
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; }

        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        public ILogger Logger { get; set; }
    }
}
