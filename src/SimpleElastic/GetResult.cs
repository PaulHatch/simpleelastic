using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// Represents the result of a get document request.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    public class GetResult<TSource>
    {
        /// <summary>
        /// Gets or sets the source document.
        /// </summary>
        [JsonProperty("_source")]
        public TSource Source { get; set; }

        /// <summary>
        /// Gets or sets the index this document came from.
        /// </summary>
        [JsonProperty("_index")]
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the document type.
        /// </summary>\
        [JsonProperty("_type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        [JsonProperty("_id")]
        public object ID { get; set; }

        /// <summary>
        /// Gets or sets the document version.
        /// </summary>
        [JsonProperty("_version")]
        public long Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the document was found.
        /// </summary>
        [JsonProperty("found")]
        public bool Found { get; set; }

        /// <summary>
        /// Gets or sets any fields for this document.
        /// </summary>
        [JsonProperty("fields")]
        public IDictionary<string, object> Fields { get; set; }
    }
}