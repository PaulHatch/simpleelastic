using Newtonsoft.Json;
using SimpleElastic.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Bucket value of an aggregation result.
    /// </summary>
    [JsonConverter(typeof(AggregationBucketConverter))]
    public sealed class AggregationBucket : AggregationBase
    {
        private IEnumerable<FlatObject> _hits;

        /// <summary>
        /// Gets the aggregation key.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// Gets the aggregation count.
        /// </summary>
        public long Count { get; internal set; }

        /// <summary>
        /// Gets the top document hits, these have been flattened.
        /// </summary>
        public IEnumerable<FlatObject> Hits
        {
            get => _hits ?? Array.Empty<FlatObject>();
            internal set => _hits = value;
        }
    }
}
