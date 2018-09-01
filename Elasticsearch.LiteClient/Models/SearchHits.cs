using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elasticsearch.LiteClient.Models
{
    internal class SearchHits<TSource>
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("hits")]
        public IEnumerable<SearchHit<TSource>> Hits { get; set; }
    }
}
