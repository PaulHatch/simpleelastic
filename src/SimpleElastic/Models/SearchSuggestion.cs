using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleElastic.Models
{
    internal class SearchSuggestion
    {
        [JsonProperty("text")]
        public string Text { get; }

        [JsonProperty("offset")]
        public int Offset { get; }

        [JsonProperty("length")]
        public int Length { get; }

        [JsonProperty("options")]
        public IEnumerable<SearchSuggestionOptions> Options { get; set; }
    }
}