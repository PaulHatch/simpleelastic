using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// The result of an update request to Elasticsearch.
    /// </summary>
    public class AcknowledgeResult
    {
        /// <summary>
        /// The acknowledged status for an Elasticsearch request.
        /// </summary>
        [JsonProperty("acknowledged")]
        public bool Acknowledged { get; set; }

        /// <summary>
        /// Any additional properties returned for this result.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> Properties { get; set; }
    }
}