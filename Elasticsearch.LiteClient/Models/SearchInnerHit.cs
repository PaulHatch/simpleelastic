using Newtonsoft.Json;
using System.Collections.Generic;

namespace Elasticsearch.LiteClient.Models
{
    
    internal class SearchInnerHit
    {
        [JsonProperty("_source", ItemConverterType = typeof(FlatConverter))]
        public IDictionary<string, object> Source { get; set; }
    }
}