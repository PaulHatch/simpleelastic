using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticsearch.LiteClient.Models
{
    internal class SearchHit<TSource>
    {
        [JsonProperty("_id")]
        public object ID { get; set; }

        [JsonProperty("_score")]
        public float? Score { get; set; }

        [JsonProperty("_source")]
        public TSource Source { get; set; }

        [JsonProperty("inner_hit")]
        public IDictionary<string, SearchInnerHits> InnerHits { get; set; }
    }
}
