using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Helper class for generating aggregation requests.
    /// </summary>
    public static class Aggregation
    {
        /// <summary>
        /// A bucket aggregation returning a form of adjacency matrix. The request
        /// provides a collection of named filter expressions, similar to the filters
        /// aggregation request. Each bucket in the response represents a non-empty
        /// cell in the matrix of intersecting  filters.
        /// </summary>
        /// <param name="filters">
        /// A map of named filter to use for this adjacency matrix aggregation, required.
        /// </param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>An object representing the requested aggregation.</returns>
        public static Map AdjacencyMatrix(IDictionary<string, object> filters, object subAggregations = null)
        {
            return new Map("adjacency_matrix",
                new Map("filters", filters ?? throw new ArgumentNullException(nameof(filters))))
            { { "aggs", subAggregations, subAggregations != null } };
        }


        /// <summary>
        /// A special single bucket aggregation that selects child documents that 
        /// have the specified type, as defined in a join field.
        /// </summary>
        /// <param name="type">The child type that should be selected, must not be null.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Children(string type, object subAggregations = null)
        {
            return new Map("children",
                new Map("type", type ?? throw new ArgumentNullException(nameof(type))))
            { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// A multi-bucket aggregation that creates composite buckets from different sources.
        /// Unlike the other multi-bucket aggregation the composite aggregation can be used to
        /// paginate all buckets from a multi-level aggregation efficiently.This aggregation
        /// provides a way to stream all buckets of a specific aggregation similarly to what
        /// scroll does for documents. The composite buckets are built from the combinations
        /// of the values extracted/created for each document and each combination is considered
        /// as a composite bucket.
        /// </summary>
        /// <param name="sources">
        /// Controls the sources that should be used to build the composite buckets. Each source
        /// is a named aggregation of either <see cref="Terms"/>, <see cref="Histogram"/>, or
        /// <see cref="DateHistogram"/>. Must not be null.
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// </param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Composite(IEnumerable<IDictionary<string, object>> sources, object subAggregations = null)
        {
            return new Map("composite",
                new Map("sources", sources ?? throw new ArgumentNullException(nameof(sources))))
            { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// This multi-bucket aggregation is similar to the normal histogram, but it can
        /// only be used with date values. Because dates are represented internally in
        /// Elasticsearch as long values, it is possible, but not as accurate, to use the
        /// normal histogram on dates as well. The main difference in the two APIs is that
        /// here the interval can be specified using date/time expressions. Time-based data
        /// requires special support because time-based intervals are not always a fixed length.
        /// </summary>
        /// <param name="field">The field to aggregate on.</param>
        /// <param name="interval">The interval to aggregate.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map DateHistogram(string field, string interval, object subAggregations = null)
        {
            return new Map("date_histogram", new Map {
                { "field", field ?? throw new ArgumentNullException(nameof(field)) },
                { "interval", interval ?? throw new ArgumentNullException(nameof(interval)) }
            }) { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// A range aggregation that is dedicated for date values. The main difference
        /// between this aggregation and the normal range aggregation is that the from
        /// and to values can be expressed in Date Math expressions, and it is also
        /// possible to specify a date format by which the from and to response fields
        /// will be returned. Note that this aggregation includes the from value and
        /// excludes the to value for each range.
        /// </summary>
        /// <param name="field">The field to aggregate on, required.</param>
        /// <param name="ranges">The date ranges to create aggregations for.</param>
        /// <param name="format">A date format.</param>
        /// <param name="keyed">True if the ranges should use key names.</param>
        /// <param name="timezone">The timezone.</param>
        /// <param name="missing">
        /// Defines how documents that are missing a value should be treated. By default
        /// they will be ignored but it is also possible to treat them as if they had a
        /// value. This is done by adding a set of fieldname : value mappings to specify
        /// default values per field.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map DateRange(
            string field,
            IDictionary<string, string> ranges,
            string format = null,
            bool? keyed = null,
            string timezone = null,
            string missing = null,
            object subAggregations = null)
        {
            return new Map("date_range", new Map
            {
                { "field", field ?? throw new ArgumentNullException(nameof(field)) },
                { "ranges", ranges ?? throw new ArgumentNullException(nameof(ranges)) },
                { "format", format, !String.IsNullOrEmpty(format) },
                { "keyed", keyed, keyed.HasValue },
                { "missing", missing, !String.IsNullOrEmpty(missing) },
                { "time_zone", timezone, !String.IsNullOrEmpty(timezone) }
            }) { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// Like the ‘sampler` aggregation this is a filtering aggregation used to limit
        /// any sub aggregations’ processing to a sample of the top-scoring documents.
        /// The diversified_sampler aggregation adds the ability to limit the number of
        /// matches that share a common value such as an "author".
        /// </summary>
        /// <param name="field">The field to aggregate on.</param>
        /// <param name="shardSize">Limits how many top-scoring documents are collected
        /// in the sample processed on each shard. The default value is 100.</param>
        /// <param name="maxDocsPerValue">An optional parameter and limits how many 
        /// documents are permitted per choice of de-duplicating value. The default 
        /// setting is "1".</param>
        /// <param name="executionHint">Optional setting which can influence the
        /// management of the values used for de-duplication. Each option will hold
        /// up to shard_size values in memory while performing de-duplication but the
        /// type of value held can be controlled as follows: (<code>map</code>) hold 
        /// field values directly, (<code>global_ordinals</code>) hold ordinals of the
        /// field as determined by the Lucene index, (<code>bytes_hash</code>) hold hashes
        /// of the field values - with potential for hash collisions.</param>
        /// <param name="script">Alternatively a script to produce a hash of the multiple
        /// values in a tags field</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map DiversifiedSampler(
            string field = null,
            int? shardSize = null,
            int? maxDocsPerValue = null,
            string executionHint = null,
            string script = null,
            object subAggregations = null)
        {
            return new Map("diversified_sampler", new Map {
                { "field", field, !String.IsNullOrEmpty(field) },
                { "max_docs_per_value", maxDocsPerValue, maxDocsPerValue.HasValue },
                { "shard_size", shardSize, shardSize.HasValue },
                { "execution_hint", executionHint, String.IsNullOrEmpty(executionHint) },
                {
                    "script",
                    new Map { { "lang", "painless" }, { "source", script } },
                    !String.IsNullOrEmpty(script)
                }
            }) { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// Defines a single bucket of all the documents in the current document set context
        /// that match a specified filter. Often this will be used to narrow down the current
        /// aggregation context to a specific set of documents.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Filter(object filter, object subAggregations = null)
        {
            return new Map("filter", filter) { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// Defines a multi bucket aggregation where each bucket is associated with a filter.
        /// Each bucket will collect all documents that match its associated filter.
        /// </summary>
        /// <param name="filters">The filters to use as aggregation buckets, the bucket results
        /// will use the names specified here.</param>
        /// <param name="otherBucket">Set to add a bucket to the response which will contain
        /// all documents that do not match any of the given filters.</param>
        /// <param name="otherBucketKey">The other bucket key.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Filters(
            IDictionary<string, object> filters, 
            bool? otherBucket = null, 
            string otherBucketKey = null,
            object subAggregations = null)
        {
            return new Map{
                { "filters", filters },
                { "other_bucket", otherBucket, otherBucket.HasValue },
                { "other_bucket_Key", otherBucketKey, !String.IsNullOrEmpty(otherBucketKey) },
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// Defines a multi bucket aggregation where each bucket is associated with a filter.
        /// Each bucket will collect all documents that match its associated filter. Use this
        /// method to define anonymous (not named) filters, the results are returned in the
        /// same order as provided in the request.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Filters(IEnumerable<object> filters, object subAggregations = null)
        {
            return new Map
            {
                { "filters", filters },
                { "aggs", subAggregations, subAggregations != null }
            };

        }

        /// <summary>
        /// Defines a multi bucket aggregation where each bucket is associated with a filter.
        /// Each bucket will collect all documents that match its associated filter. Use this
        /// method to define anonymous (not named) filters, the results are returned in the
        /// same order as provided in the request.
        /// </summary>
        /// <param name="filters">The filters.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Filters(params object[] filters)
            => Filter(filters.AsEnumerable(), null);

        /// <summary>
        /// A multi-bucket aggregation that works on geo_point fields and conceptually works
        /// very similar to the range aggregation. The user can define a point of origin and
        /// a set of distance range buckets. The aggregation evaluate the distance of each
        /// document value from the origin point and determines the buckets it belongs to
        /// based on the ranges (a document belongs to a bucket if the distance between the
        /// document and the origin falls within the distance range of the bucket).
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="origin">The origin point can accept all formats supported by the
        /// geo_point type; object format such as <code>{ "lat" : 52.3760, "lon" : 4.894 }</code>,
        /// this is the safest format as it is the most explicit about the lat &amp; lon values,
        /// string format such as <code>"52.3760, 4.894"</code> where the first number is the lat
        /// and the second is the lon, or in array format such as <code>[4.894, 52.3760]</code>
        /// which is based on the GeoJson standard and where the first number is the lon and the
        /// second one is the lat.</param>
        /// <param name="ranges">The ranges to return distances for, each value may have a
        /// <code>to</code> and/or <code>from</code> field, and if <code>keyed</code> has been set,
        /// should have a <code>key</code> field with a unique key for the range as well.</param>
        /// <param name="keyed">True if the ranges should use key names.</param>
        /// <param name="unit">The unit type. By default, the distance unit is m (meters) but it
        /// can also accept: mi (miles), in (inches), yd (yards), km (kilometers), cm (centimeters),
        /// mm (millimeters).</param>
        /// <param name="distanceType">Specify the distance type. There are two distance calculation
        /// modes: arc (the default), and plane. The arc calculation is the most accurate. The plane
        /// is the fastest but least accurate. Consider using plane when your search context is
        /// "narrow", and spans smaller geographical areas (~5km). plane will return higher error
        /// margins for searches across very large areas (e.g. cross continent search).</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map GeoDistance(
            string field, 
            object origin, 
            IEnumerable<object> ranges = null, 
            bool? keyed = null,
            string unit = null, 
            string distanceType = null,
            object subAggregations = null)
        {
            return new Map("geo_distance", new Map {
                { "field", field ?? throw new ArgumentNullException(nameof(field)) },
                { "origin", origin ?? throw new ArgumentNullException(nameof(origin)) },
                { "unit", unit, !String.IsNullOrEmpty(unit) },
                { "distance_type", distanceType, !String.IsNullOrEmpty(distanceType) },
                { "keyed", keyed, keyed.HasValue },
            }) { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// A multi-bucket aggregation that works on geo_point fields and groups points into
        /// buckets that represent cells in a grid. The resulting grid can be sparse and only
        /// contains cells that have matching data. Each cell is labeled using a geohash which
        /// is of user-definable precision.
        /// </summary>
        /// <param name="field">The field to aggregate on, must be a <code>geo_point</code> 
        /// type field.</param>
        /// <param name="precision">The precision level between 1-12. High precision geohashes
        /// have a long string length and represent cells that cover only a small area while low
        /// precision geohashes have a short string length and represent cells that each cover a
        /// large area.<param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map GeoHashgrid(string field, int precision, object subAggregations = null)
        {
            return new Map("geohash_grid", new Map {
                { "field", field ?? throw new ArgumentNullException(nameof(field)) },
                { "precision", precision }
            }) { { "aggs", subAggregations, subAggregations != null } };
        }

        /// <summary>
        /// Defines a single bucket of all the documents within the search execution context. This context
        /// is defined by the indices and the document types you’re searching on, but is not influenced by
        /// the search query itself. Global aggregators can only be placed as top level aggregators because
        /// it doesn’t make sense to embed a global aggregator within another bucket aggregator.
        /// </summary>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>An object representing the requested aggregation.</returns>
        public static Map Global(object subAggregations = null)
        {
            return new Map("global", new object())
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// A multi-bucket values source based aggregation that can be applied on numeric values extracted
        /// from the documents. It dynamically builds fixed size (a.k.a. interval) buckets over the values.
        /// For example, if the documents have a field that holds a price (numeric), we can configure this
        /// aggregation to dynamically build buckets with interval 5 (in case of price it may represent $5).
        /// </summary>
        /// <param name="field">The field to aggregate on.</param>
        /// <param name="interval">The interval of the histogram.</param>
        /// <param name="offset">Overrides the offset, by default the bucket keys start with 0 and then
        /// continue in even spaced steps of interval.</param>
        /// <param name="order">By default the returned buckets are sorted by their key ascending, though
        /// the order behavior can be controlled using the order setting.</param>
        /// <param name="minDocCount">The minimum document count.</param>
        /// <param name="missing">Defines how documents that are missing a value should be treated. By default
        /// they will be ignored but it is also possible to treat them as if they had a value.</param>
        /// <param name="keyed">By default, the buckets are returned as an ordered array. Set this value to
        /// true to request the response as a hash instead keyed by the buckets keys:</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        /// <exception cref="ArgumentNullException">field
        /// or
        /// interval</exception>
        /// n&gt;
        public static Map Histogram(
            string field, 
            object interval, 
            object offset = null, 
            object order = null, 
            int? minDocCount = null, 
            object missing = null,
            bool? keyed = null,
            object subAggregations = null)
        {
            return new Map("histogram", new Map {
                { "field", field ?? throw new ArgumentNullException(nameof(field)) },
                { "interval", interval ?? throw new ArgumentNullException(nameof(interval)) },
                { "offset", offset , offset != null },
                { "order", order , order != null },
                { "missing", missing, missing != null },
                { "keyed", keyed, keyed.HasValue },
                { "min_doc_count", minDocCount, minDocCount.HasValue }
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// Range aggregation for IP addresses.
        /// </summary>
        /// <param name="field">The field to aggregate on.</param>
        /// <param name="ranges">The ranges to aggregate, these can include <code>min</code> and/or 
        /// <code>max</code> values or a <code>mask</code>, and if <code>keyed</code> has been set,
        /// should have a <code>key</code> field with a unique key for the range as well..</param>
        /// <param name="keyed">True if the ranges should use key names.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map IPRange(string field, IEnumerable<object> ranges, bool? keyed, object subAggregations = null)
        {
            return new Map("ip_range", new Map
            {
                { "field", field ?? throw new ArgumentNullException(nameof(field)) },
                { "ranges", ranges },
                { "keyed", keyed, keyed.HasValue },
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// A field data based single bucket aggregation, that creates a bucket of all documents in the
        /// current document set context that are missing a field value (effectively, missing a field or
        /// having the configured NULL value set). This aggregator will often be used in conjunction with
        /// other field data bucket aggregators (such as ranges) to return information for all the
        /// documents that could not be placed in any of the other buckets due to missing field data values.
        /// </summary>
        /// <param name="field">The field to check for missing values.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Missing(string field, object subAggregations = null)
        {
            return new Map("missing", new Map("field", field))
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// A special single bucket aggregation that enables aggregating nested documents.
        /// </summary>
        /// <param name="path">The path of the nested document field.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Nested(string path, object subAggregations = null)
        {
            return new Map("nested", new Map
            {
                { "path", path ?? throw new ArgumentNullException(nameof(path)) }
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// A multi-bucket value source based aggregation that enables the user to define a set of ranges
        /// - each representing a bucket. During the aggregation process, the values extracted from each
        /// document will be checked against each bucket range and "bucket" the relevant/matching document.
        /// Note that this aggregation includes the from value and excludes the to value for each range.
        /// </summary>
        /// <param name="field">The field to aggregate on.</param>
        /// <param name="keyed">True if the ranges should use key names.</param>
        /// <param name="script">The script.</param>
        /// <param name="subAggregations">The sub aggregations.</param>
        /// <returns></returns>
        public static Map Range(
            string field = null,
            bool? keyed = null,
            string script = null,
            object subAggregations = null)
        {
            return new Map("range", new Map
            {
                { "field", field, !String.IsNullOrEmpty(field) },
                { "keyed", keyed, keyed.HasValue },
                {
                    "script",
                    new Map { { "lang", "painless" }, { "source", script } },
                    !String.IsNullOrEmpty(script)
                }
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// A special single bucket aggregation that enables aggregating on parent docs from nested 
        /// documents. Effectively this aggregation can break out of the nested block structure and link
        /// to other nested structures or the root document, which allows nesting other aggregations that
        /// aren’t part of the nested object in a nested aggregation.
        /// </summary>
        /// <param name="path">Defines to what nested object field should be joined back. The default is
        /// empty, which means that it joins back to the root / main document level. The path cannot
        /// contain a reference to a nested object field that falls outside the nested aggregation’s
        /// nested structure a reverse_nested is in.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map ReverseNested(string path = null, object subAggregations = null)
        {
            return new Map("reverse_nested", new Map
            {
                { "path", path, !String.IsNullOrEmpty(path) }
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// A filtering aggregation used to limit any sub aggregations' processing to a sample of the
        /// top-scoring documents.
        /// </summary>
        /// <param name="shardSize">Limits how many top-scoring documents are collected in the sample
        /// processed on each shard. The default value is 100.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Sampler(int? shardSize = null, object subAggregations = null)
        {
            return new Map("sampler", new Map
            {
                {"shard_size", shardSize, shardSize.HasValue }
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /*

            TODO: Add support for significant terms and significant text


        /// <summary>
        /// An aggregation that returns interesting or unusual occurrences of terms in a set.
        /// </summary>
        /// <param name="field">The field to aggregate terms for.</param>
        /// <param name="jlh">True to enable JHL. The scores are derived from the doc frequencies in
        /// foreground and background sets. The absolute change in popularity (foregroundPercent -
        /// backgroundPercent) would favor common terms whereas the relative change in popularity
        /// (foregroundPercent/ backgroundPercent) would favor rare terms. Rare vs common is essentially
        /// a precision vs recall balance and so the absolute and relative changes are multiplied to
        /// provide a sweet spot between precision and recall..</param>
        /// <param name="chiSquare">True to enable Chi square as described in "Information
        /// Retrieval", Manning et al., Chapter 13.5.2.</param>
        /// <param name="gnd">The GND.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map SignificantTerms(
            string field, 
            bool? jlh = null,
            bool? chiSquare = null,
            bool? gnd = null,
            object subAggregations = null)
        {
            return new Map("significant_terms", new Map
            {
                { "field", field },
                { "jlh", new object(), jlh == true },
                { "chi_square", new object(), chiSquare == true },
                { "gnd", new object(), jlh == true },

            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>An object representing the requested aggregation.</returns>
        public static Map SignificantText(object subAggregations = null)
        {
            return new Map("", new Map
            {
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }

        */

        /// <summary>
        /// A multi-bucket value source based aggregation where buckets are dynamically built - one per
        /// unique value.
        /// </summary>
        /// <param name="field">The field to aggregate terms for.</param>
        /// <param name="size">The maximum number of results to return.</param>
        /// <param name="order">Overrides the order to return results in.</param>
        /// <param name="minDocCount">The minimum document count to return values for.</param>
        /// <param name="showTermDocCountError">If true shows an error value for each term returned by the
        /// aggregation which represents the worst case error in the document count and can be useful when
        /// deciding on a value for the shard_size parameter. This is calculated by summing the document
        /// counts for the last term returned by all shards which did not return the term.</param>
        /// <param name="executionHint">Set the execution hint for the query, supported values are <code>map</code>
        /// and <code>global_ordinals</code>. <code>global_ordinals</code> is the default option for keyword
        /// field, it uses global ordinals to allocates buckets dynamically so memory usage is linear to the
        /// number of values of the documents that are part of the aggregation scope. <code>map</code> should
        /// only be considered when very few documents match a query. Otherwise the ordinals-based execution
        /// mode is significantly faster. By default, map is only used when running an aggregation on scripts,
        /// since they don’t have ordinals. </param>
        /// <param name="script">A script to use to calculate values.</param>
        /// <param name="include">A regular expression to filter terms to include.</param>
        /// <param name="exclude">A regular expression to filter terms to  exclude.</param>
        /// <param name="subAggregations">An optional set of sub aggregations for this aggregation.</param>
        /// <returns>
        /// An object representing the requested aggregation.
        /// </returns>
        public static Map Terms(
            string field = null,
            int? size = null,
            object order = null,
            int? minDocCount = null,
            bool? showTermDocCountError = null,
            string executionHint = null,
            string script = null,
            string include = null,
            string exclude = null,
            object subAggregations = null)
        {
            return new Map("terms", new Map
            {
                { "field", field, !String.IsNullOrEmpty(field) },
                { "size", size, size.HasValue },
                { "order", order, order != null },
                { "min_doc_count", minDocCount, minDocCount.HasValue },
                { "include", include, !String.IsNullOrEmpty(include) },
                { "include", exclude, !String.IsNullOrEmpty(exclude) },
                { "execution_hint", executionHint, !String.IsNullOrEmpty(executionHint) },
                { "show_term_doc_count_error", showTermDocCountError, showTermDocCountError.HasValue },
                {
                    "script",
                    new Map { { "lang", "painless" }, { "source", script } },
                    !String.IsNullOrEmpty(script)
                }
            })
            {
                { "aggs", subAggregations, subAggregations != null }
            };
        }
    }
}
