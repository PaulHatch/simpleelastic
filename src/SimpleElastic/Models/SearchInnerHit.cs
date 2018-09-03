using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleElastic.Models
{
    
    internal class SearchInnerHit
    {
        [JsonProperty("_source")]
        public FlatObject Source { get; set; }
    }
}