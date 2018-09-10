using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Represents the result of a bulk action request.
    /// </summary>
    public class BulkActionResult
    {
        /// <summary>
        /// Gets the time the request took on elasticsearch.
        /// </summary>
        [JsonProperty("took"), JsonConverter(typeof(TimeSpanConveter))]
        public TimeSpan Took { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether there were any errors for this request.
        /// </summary>
        [JsonProperty("errors")]
        public bool HasErrors { get; internal set; }

        /// <summary>
        /// Gets or sets the specific result for each of the requested actions.
        /// </summary>
        [JsonProperty("Items")]
        public IEnumerable<BulkActionResultItem> Items { get; set; } 
    }
}
