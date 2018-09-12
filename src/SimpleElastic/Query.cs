using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Helper class for generating queries.
    /// </summary>
    public class Query
    {
        /// <summary>
        /// /// Create a term filter which finds documents that contain the exact term specified
        /// in the inverted index.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The field value to match exactly.</param>
        /// <param name="boost">
        /// Optional boost parameter to give this term query a higher relevance score than another query.
        /// </param>
        /// <returns>An object representing a term query.</returns>
        public object Term(string field, object value, float? boost = null)
        {
            if (boost.HasValue)
            {
                return new { term = new Map(field, new { value = value, boost = boost.Value }) };
            }
            else
            {
                return new { term = new Map(field, value) };
            }
        }

        /// <summary>
        /// Create a terms filter which filters documents that have fields that match any of the
        /// provided terms (not analyzed).
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="values">The field values to match exactly.</param>
        /// <param name="boost">
        /// Optional boost parameter to give this terms query a higher relevance score than another query.
        /// </param>
        /// <returns>An object representing a terms query.</returns>
        public object Terms(string field, IEnumerable<object> values, float? boost = null)
        {
            if (boost.HasValue)
            {
                return new { terms = new Map(field, new { value = values, boost = boost.Value }) };
            }
            else
            {
                return new { terms = new Map(field, values) };
            }
        }

        /// <summary>
        /// Create a terms filter which uses specified parameters to reference another document
        /// for source values, the values for the terms filter will be fetched from a field in a
        /// document with the specified id in the specified type and index. Internally a get
        /// request is executed to fetch the values from the specified path.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="index">The index to fetch the term values from.</param>
        /// <param name="id">The id of the document to fetch the term values from.</param>
        /// <param name="path">The field specified as path to fetch the actual values for the terms filter.</param>
        /// <param name="type">The type to fetch the term values from.</param>
        /// <param name="boost">Optional boost parameter to give this terms query a higher relevance score than another query.</param>
        /// <param name="routing">A custom routing value to be used when retrieving the external terms doc.</param>
        /// <returns>
        /// An object representing a terms query.
        /// </returns>
        public object Terms(string field, string index, object id, string path, string type = null, float? boost = null, string routing = null)
        {
            var terms = new Map()
            {
                { "index", index ?? throw new ArgumentNullException(nameof(index)) },
                { "id", id ?? throw new ArgumentNullException(nameof(id)) },
                { "path", path ?? throw new ArgumentNullException(nameof(path)) }
            };

            if (type != null)
            {
                terms["type"] = type;
            }

            if (boost.HasValue)
            {
                terms["boost"] = boost.Value;
            }

            if (routing != null)
            {
                terms["routing"] = routing;
            }

            return new { terms = new Map(field, terms) };

        }
    }
}
