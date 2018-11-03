using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Base class for aggregation results.
    /// </summary>
    public abstract class AggregationBase
    {
        private IDictionary<string, AggregationResult> _subAggregations;

        /// <summary>
        /// Gets the sub aggregation results for this aggregation.
        /// </summary>
        /// <remarks>
        /// Accessing this property is not thread safe.
        /// </remarks>
        public IReadOnlyDictionary<string, AggregationResult> SubAggregations
        {
            get
            {
                return (IReadOnlyDictionary<string, AggregationResult>)_subAggregations
                    ?? EmptyDictionary<string, AggregationResult>.Instance;
            }
        }

        internal void AddAggregation(string name, AggregationResult aggregation)
        {
            if (_subAggregations == null)
            {
                _subAggregations = new Dictionary<string, AggregationResult>();
            }
            _subAggregations.Add(name, aggregation);
        }
    }
}
