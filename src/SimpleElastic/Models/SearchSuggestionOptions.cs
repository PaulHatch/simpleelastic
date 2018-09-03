using Newtonsoft.Json;

namespace SimpleElastic.Models
{
    internal class SearchSuggestionOptions
    {
        [JsonProperty("text")]
        public string Text { get; }

        [JsonProperty("highlighted")]
        public string Highlighted { get; }

        [JsonProperty("score")]
        public float Score { get; }
    }
}