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
        private IDictionary<string, Aggregation> _subAggregations;

        /// <summary>
        /// Gets the sub aggregation results for this aggregation.
        /// </summary>
        /// <remarks>
        /// Accessing this property is not thread safe.
        /// </remarks>
        public IReadOnlyDictionary<string, Aggregation> SubAggregations
        {
            get
            {
                return (IReadOnlyDictionary<string, Aggregation>)_subAggregations
                    ?? EmptyDictionary<string, Aggregation>.Instance;
            }
        }

        internal void AddAggregation(string name, Aggregation aggregation)
        {
            if (_subAggregations == null)
            {
                _subAggregations = new Dictionary<string, Aggregation>();
            }
            _subAggregations.Add(name, aggregation);
        }
    }
}
