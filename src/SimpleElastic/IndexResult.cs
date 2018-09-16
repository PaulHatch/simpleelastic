using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleElastic.Converters;
using System;
using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// Represents information from an index get request.
    /// </summary>
    public class IndexResult
    {
        /// <summary>
        /// Gets or sets the aliases for this request.
        /// </summary>
        [JsonProperty("aliases")]
        public IDictionary<string, JObject> Aliases { get; set; }

        /// <summary>
        /// Gets or sets the property mapping for this index for each document type.
        /// </summary>
        [JsonProperty("mappings")]
        public IDictionary<string, JObject> Mapping { get; set; }

        /// <summary>
        /// Gets or sets the settings for this index.
        /// </summary>
        [JsonProperty("settings")]
        public IDictionary<string, SettingsResult> Settings { get; set; }

        /// <summary>
        /// Represents the settings from an index get request.
        /// </summary>
        public class SettingsResult
        {
            /// <summary>
            /// Gets or sets the creation date for the index.
            /// </summary>
            [JsonProperty("creation_date"), JsonConverter(typeof(UnixTimeConverter))]
            public DateTimeOffset CreationDate { get; set; }

            /// <summary>
            /// Gets or sets the number of shards for this index.
            /// </summary>
            [JsonProperty("number_of_shards")]
            public int NumberOfShards { get; set; }

            /// <summary>
            /// Gets or sets the number of replicas for this index.
            /// </summary>
            [JsonProperty("number_of_replicas")]
            public int NumberOfReplicas { get; set; }

            /// <summary>
            /// Gets or sets the index UUID.
            /// </summary>
            [JsonProperty("uuid")]
            public string Uuid { get; set; }

            /// <summary>
            /// Gets or sets any addition properties returned for the settings.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> Properties { get; set; }
        }
    }
}