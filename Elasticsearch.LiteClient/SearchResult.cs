using System.Collections.Generic;
using Elasticsearch.LiteClient.Models;

namespace Elasticsearch.LiteClient
{
    public class SearchResult<TDocument>
    {
        public long Total { get; internal set; }
        internal IEnumerable<SearchHit<TDocument>> Hits { get; set; }
    }
}