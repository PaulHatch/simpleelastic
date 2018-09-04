using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// Aggregation result of a query.
    /// </summary>
    [JsonConverter(typeof(AggregationConverter))]
    public sealed class Aggregation : AggregationBase
    {
        private IEnumerable<AggregationBucket> _buckets;

        /// <summary>
        /// Gets the results for this aggregation.
        /// </summary>
        public IEnumerable<AggregationBucket> Buckets
        {
            get => _buckets ?? Array.Empty<AggregationBucket>();
            internal set => _buckets = value;
        }

        /// <summary>
        /// Gets the document count error upper bound.
        /// </summary>
        public long DocumentCountErrorUpperBound { get; internal set; }

        /// <summary>
        /// Gets the sum other document count.
        /// </summary>
        public long SumOtherDocumentCount { get; internal set; }
    }
}