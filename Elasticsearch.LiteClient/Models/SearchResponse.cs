using Newtonsoft.Json;

namespace Elasticsearch.LiteClient.Models
{
    internal class SearchResponse<TSource>
    {
        [JsonProperty("hits")]
        public SearchHits<TSource> Hits { get; set; }

        [JsonProperty("_scroll_id")]
        public string ScrollID { get; set; }
    }
}