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
        /// Create a term query which finds documents that contain the exact term specified
        /// in the inverted index.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The value to match exactly.</param>
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
        /// Create a terms query which filters documents that have fields that match any of the
        /// provided terms (not analyzed).
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="values">The values to match exactly.</param>
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
        /// Create a terms query which uses specified parameters to reference another document
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
                { "path", path ?? throw new ArgumentNullException(nameof(path)) },
                { "type", type, type != null },
                { "boost", boost, boost.HasValue },
                { "routing", routing, routing != null }
            };

            return new { terms = new Map(field, terms) };
        }

        /// <summary>
        /// Create a terms query which filters documents that have fields that match any of the
        /// provided terms (not analyzed).
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="values">The values to match exactly.</param>
        /// <param name="boost">
        /// Optional boost parameter to give this terms query a higher relevance score than another query.
        /// </param>
        /// <returns>An object representing a terms query.</returns>
        public object Range(
            string field,
            string gt = null,
            string lt = null,
            string gte = null,
            string lte = null,
            float? boost = null,
            string timeZone = null,
            string format = null)
        {
            var range = new Map
            {
                { "gt", gt, gt != null },
                { "lt", lt, lt != null },
                { "gte", gte, gte != null },
                { "lte", lte, lte != null },
                { "boost", boost, boost.HasValue },
                { "time_zone", timeZone, timeZone != null },
                { "format", format, format != null }
            };

            return new { terms = new Map(field, range) };
        }

        /// <summary>
        /// Creates a query which returns documents that have at least one non-null value in the original field
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <returns>An object representing an exists query.</returns>
        public object Exists(string field)
        {
            return new { exists = new { field = field } };

        }

        /// <summary>
        /// Create a prefix query which finds documents that contain the exact term specified
        /// in the inverted index.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The value to prefix match.</param>
        /// <param name="boost">Optional boost parameter to give this term query a higher relevance score than another query.</param>
        /// <param name="rewrite">Optional rewrite parameter allow control of how the query will get rewritten.</param>
        /// <returns>
        /// An object representing a prefix query.
        /// </returns>
        public object Prefix(string field, string value, float? boost = null, string rewrite = null)
        {
            return new
            {
                prefix = new Map(field, new Map
                {
                    { "value", value },
                    { "boost", boost.Value, boost.HasValue },
                    { "rewrite", rewrite, rewrite != null }
                })
            };
        }

        /// <summary>
        /// Create a wildcard query which matches documents that have fields matching a wildcard expression (not analyzed). Supported
        /// wildcards are *, which matches any character sequence (including the empty one), and ?, which matches any single character.
        /// Note that this query can be slow, as it needs to iterate over many terms. In order to prevent extremely slow wildcard queries,
        /// a wildcard term should not start with one of the wildcards * or ?.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The wildcard expression.</param>
        /// <param name="boost">Optional boost parameter to give this term query a higher relevance score than another query.</param>
        /// <param name="rewrite">Optional rewrite parameter allow control of how the query will get rewritten.</param>
        /// <returns>
        /// An object representing a wildcard query.
        /// </returns>
        public object Wildcard(string field, string value, float? boost = null, string rewrite = null)
        {
            return new
            {
                wildcard = new Map(field, new Map
                {
                    { "value", value },
                    { "boost", boost.Value, boost.HasValue },
                    { "rewrite", rewrite, rewrite != null }
                })
            };
        }

        /// <summary>
        /// Create a regex query which allows you to use regular expression term queries. The "term queries" means that Elasticsearch
        /// will apply the regexp to the terms produced by the tokenizer for that field, and not to the original text of the field.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The regular expression for the query.</param>
        /// <param name="boost">Optional boost parameter to give this term query a higher relevance score than another query.</param>
        /// <param name="flags">
        /// Optional flags, possible flags are ALL (default), ANYSTRING, COMPLEMENT, EMPTY, INTERSECTION, INTERVAL, or NONE.
        /// </param>
        /// <returns>
        /// An object representing a regex query.
        /// </returns>
        public object Regex(string field, string value, float? boost = null, string flags = null)
        {
            return new
            {
                regex = new Map(field, new Map
                {
                    { "value", value },
                    { "boost", boost.Value, boost.HasValue },
                    { "flags", flags, flags != null }
                })
            };
        }

        /// <summary>
        /// Create a fuzzy query which generates matching terms that are within the maximum edit distance specified in fuzziness
        /// and then checks the term dictionary to find out which of those generated terms actually exist in the index. The final
        /// query uses up to max_expansions matching terms.
        /// </summary>
        /// <param name="field">The field to match.</param>
        /// <param name="value">The value to fuzzy match on.</param>
        /// <param name="boost">Optional boost parameter to give this term query a higher relevance score than another query.</param>
        /// <param name="fuzziness">The maximum edit distance. Defaults to AUTO.</param>
        /// <param name="prefixLength">The number of initial characters which will not be “fuzzified”. This helps to reduce the number of terms which must be
        /// examined. Defaults to 0.</param>
        /// <param name="maxExpansions">The maximum number of terms that the fuzzy query will expand to. Defaults to 50.</param>
        /// <param name="transpositions">Whether fuzzy transpositions (ab -&gt; ba) are supported. Default is false.</param>
        /// <returns>
        /// An object representing a fuzzy query.
        /// </returns>
        public object Fuzzy(
            string field, 
            string value, 
            float? boost = null, 
            object fuzziness = null,
            int? prefixLength = null,
            int? maxExpansions = null,
            bool? transpositions = null)
        {
            return new
            {
                fuzzy = new Map(field, new Map
                {
                    { "value", value },
                    { "boost", boost.Value, boost.HasValue },
                    { "fuzziness", fuzziness, fuzziness != null },
                    { "prefix_length", prefixLength, prefixLength.HasValue },
                    { "max_expansions", prefixLength, maxExpansions.HasValue },
                    { "transpositions", prefixLength, transpositions.HasValue }
                })
            };
        }

        /// <summary>
        /// Creates an ID query which filters documents that only have the provided IDs.
        /// </summary>
        /// <param name="ids">The document IDs to filter by.</param>
        /// <returns>An object representing an IDs query.</returns>
        public object Ids(IEnumerable<object> ids)
        {
            return new { ids = new { values = ids } };
        }
    }
}
