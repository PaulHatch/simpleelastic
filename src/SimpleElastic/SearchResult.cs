using SimpleElastic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleElastic
{
    /// <summary>
    /// Represents the result of a search request.
    /// </summary>
    /// <typeparam name="TDocument">The type of the search source document.</typeparam>
    public class SearchResult<TDocument>
    {
        internal SearchResult(
            IEnumerable<TDocument> hits,
            long total,
            IReadOnlyDictionary<string, Aggregation> aggregations,
            IReadOnlyDictionary<string, SuggestionResult> suggestions)
        {
            Hits = hits;
            Total = total;
            Aggregations = aggregations;
            Suggestions = suggestions;
        }

        /// <summary>
        /// Gets the total documents returned by the query.
        /// </summary>
        public long Total { get; }

        /// <summary>
        /// Gets the documents returned by the search.
        /// </summary>
        public IEnumerable<TDocument> Hits { get; }

        /// <summary>
        /// Gets the aggregations for this search.
        /// </summary>
        public IReadOnlyDictionary<string, Aggregation> Aggregations { get; }

        /// <summary>
        /// Gets the suggestions from this search.
        /// </summary>
        public IReadOnlyDictionary<string, SuggestionResult> Suggestions { get; }
    }
}