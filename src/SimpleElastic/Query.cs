using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    { "boost", boost, boost.HasValue },
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
                    { "boost", boost, boost.HasValue },
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
                    { "boost", boost, boost.HasValue },
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
                    { "boost", boost, boost.HasValue },
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

        /// <summary>
        /// Creates an constant score query that wraps another query and simply returns a
        /// constant score equal to the query boost for every document in the filter.
        /// </summary>
        /// <param name="filter">The filter, filter clauses are executed in filter context,
        /// meaning that scoring is ignored and clauses are considered for caching..</param>
        /// <param name="boost">The boost value to assign to matching documents.</param>
        /// <returns>
        /// An object representing a constant score query.
        /// </returns>
        public object ConstantScore(object filter, float boost)
        {
            return new { constant_score = new { filter = filter, boost = boost } };
        }

        /// <summary>
        /// Creates a bool query which matches documents matching boolean combinations of other queries.
        /// </summary>
        /// <param name="must">The clause (query) must appear in matching documents and will contribute to the score.</param>
        /// <param name="mustNot">The clause (query) must not appear in the matching documents. Clauses are executed in filter
        /// context meaning that scoring is ignored and clauses are considered for caching. Because scoring is ignored, a score
        /// of 0 for all documents is returned.</param>
        /// <param name="should">The clause (query) should appear in the matching document. If the bool query is in a query
        /// context and has a must or filter clause then a document will match the bool query even if none of the should queries
        /// match. In this case these clauses are only used to influence the score. If the bool query is a filter context or has
        /// neither must or filter then at least one of the should queries must match a document for it to match the bool query.
        /// This behavior may be explicitly controlled by settings the minimumShouldMatch parameter.</param>
        /// <param name="filter">The clause (query) must appear in matching documents. However unlike must the score of the query
        /// will be ignored. Filter clauses are executed in filter context, meaning that scoring is ignored and clauses are 
        /// considered for caching.</param>
        /// <param name="minimumShouldMatch">Controls how queries matches are counted.</param>
        /// <param name="boost">The boost value for this query.</param>
        /// <returns>
        /// An object representing a bool query.
        /// </returns>
        public object Bool(
            IEnumerable<object> must = null,
            IEnumerable<object> mustNot = null,
            IEnumerable<object> should = null,
            IEnumerable<object> filter = null,
            object minimumShouldMatch = null,
            float? boost = null)
        {
            return new
            {
                @bool = new Map
                {
                    { "must", must, must?.Any() == true },
                    { "must_not", mustNot, mustNot?.Any() == true },
                    { "should", should, should?.Any() == true },
                    { "filter", filter, filter?.Any() == true },
                    { "minimum_should_match", minimumShouldMatch, minimumShouldMatch != null },
                    { "boost", boost, boost.HasValue }
                }
            };
        }

        private static int Count<T>(IEnumerable<T> enumerable)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return 0;
                }
                if (!enumerator.MoveNext())
                {
                    return 1;
                }
                return 2;
            }
        }

        /// <summary>
        /// Creates a logical OR query using a bool 'should' query, if no queries are provided, a match all will be returned,
        /// if only one query is provided the bool query will be skipped and just that query will be returned.
        /// </summary>
        /// <param name="queries">The queries to combine.</param>
        /// <returns>A query representing the specified operation.</returns>
        public static object Or(params object[] queries)
        {
            return Or((IEnumerable<object>)queries);
        }


        /// <summary>
        /// Creates a logical OR query using a bool 'should' query, if no queries are provided, a match all will be returned,
        /// if only one query is provided the bool query will be skipped and just that query will be returned.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <returns></returns>
        public static object Or(IEnumerable<object> queries)
        {
            switch (Count(queries))
            {
                case 0: return new { match_all = new { } };
                case 1: return queries.ElementAt(0);
                default: return new { @bool = new { should = queries, minimum_should_match = 1 } };
            }
        }

        /// <summary>
        /// Creates a logical And query using a bool 'should' query, if no queries are provided, a "not match all"
        /// will be returned to ensure that the query matches no documents are returned, if only one query is 
        /// provided the bool query will be skipped and just that query will be returned.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <returns></returns>
        public static object And(params object[] queries)
        {
            return Or((IEnumerable<object>)queries);
        }


        /// <summary>
        /// Creates a logical And query using a bool 'should' query, if no queries are provided, a "not match all"
        /// will be returned to ensure that the query matches no documents are returned, if only one query is 
        /// provided the bool query will be skipped and just that query will be returned.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <returns></returns>
        public static object And(IEnumerable<object> queries)
        {
            switch (Count(queries))
            {
                case 0: return new { @bool = new { must_not = new { match_all = new { } } } };
                case 1: return queries.ElementAt(0);
                default: return new { @bool = new { must = queries } };
            }
        }
    }
}
