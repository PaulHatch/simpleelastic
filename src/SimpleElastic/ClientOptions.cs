using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Specifies options for client creation.
    /// </summary>
    public class ClientOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientOptions"/> class.
        /// </summary>
        public ClientOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientOptions"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public ClientOptions(Uri host)
        {
            HostProvider = new SingleHostProvider(host);
        }

        /// <summary>
        /// Initializes a new <see cref="ClientOptions"/> with a host provider
        /// created for the specified hosts.
        /// </summary>
        /// <param name="hosts">The hosts to connect to.</param>
        public ClientOptions(params Uri[] hosts) : this(hosts.AsEnumerable())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ClientOptions"/> with a host provider
        /// created for the specified hosts.
        /// </summary>
        /// <param name="hosts">The hosts to connect to.</param>
        public ClientOptions(IEnumerable<Uri> hosts)
        {
            switch (hosts?.Count() ?? throw new ArgumentNullException(nameof(hosts)))
            {
                case 0:
                    break;
                case 1:
                    HostProvider = new SingleHostProvider(hosts.Single());
                    break;
                default:
                    HostProvider = new HostPoolProvider(hosts);
                    break;
            }
        }

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
