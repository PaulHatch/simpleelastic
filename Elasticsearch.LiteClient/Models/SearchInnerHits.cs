using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elasticsearch.LiteClient.Models
{
    internal class SearchInnerHits
    {
        [JsonProperty("hits")]
        public IEnumerable<SearchInnerHit> Hits { get; set; }
    }
}