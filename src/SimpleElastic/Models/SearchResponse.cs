using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleElastic.Models
{
    internal class SearchResponse<TSource>
    {
        [JsonProperty("hits")]
        public SearchHits<TSource> Hits { get; set; }

        [JsonProperty("_scroll_id")]
        public string ScrollID { get; set; }

        [JsonProperty("took")]
        public long Took { get; set; }

        [JsonProperty("aggregations")]
        public Dictionary<string, AggregationResult> Aggregations { get; set; }

        [JsonProperty("suggest")]
        public Dictionary<string, IEnumerable<SearchSuggestion>> Suggestions { get; set; }
    }
}