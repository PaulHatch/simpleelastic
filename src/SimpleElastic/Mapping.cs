using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{

    /// <summary>
    /// Helper for creating properties for index mappings. Inherits from <see cref="Map"/>.
    /// </summary>
    public class Property : Map
    {
        /// <summary>
        /// Helper for creating properties for index mappings. Inherits from <see cref="Map"/>.
        /// </summary>
        /// <param name="type">The Elasticsearch type of this property.</param>
        public Property(string type)
        {
            Add("type", type);
        }

        // Quick type create properties

        /// <summary>
        /// Begins a new text field to index full-text values, such as the body of an email or the
        /// description of a product. These fields are analyzed, that is they are passed through an analyzer
        /// to convert the string into a list of individual terms before being indexed. The analysis process
        /// allows Elasticsearch to search for individual words within each full text field. Text fields are
        /// not used for sorting and seldom used for aggregations (although the significant text aggregation
        /// is a notable exception).
        /// </summary>
        public static Property Text => new Property("text");

        /// <summary>
        /// Begins a new field to index structured content such as email addresses, hostnames, status codes,
        /// zip codes or tags. They are typically used for filtering (Find me all blog posts where
        /// status is published), for sorting, and for aggregations.Keyword fields are only
        /// searchable by their exact value. If you need to index full text content such as email
        /// bodies or product descriptions, it is likely that you should rather use a text field.
        /// </summary>
        public static Property Keyword => new Property("keyword");

        /// <summary>
        /// Begins a new date type property.
        /// </summary>
        public static Property Date => new Property("date");

        /// <summary>
        /// Begins a new 64 bit integer type property.
        /// </summary>
        public static Property Long => new Property("long");

        /// <summary>
        /// Begins a new 32 bit integer type property.
        /// </summary>
        public static Property Integer => new Property("integer");

        /// <summary>
        /// Begins a new 16 bit integer type property.
        /// </summary>
        public static Property Short => new Property("short");

        /// <summary>
        /// Begins a new 8 bit integer type property.
        /// </summary>
        public static Property Byte => new Property("long");

        /// <summary>
        /// Begins a new 64 bit floating point type property.
        /// </summary>
        public static Property Double => new Property("double");

        /// <summary>
        /// Begins a new 32 bit floating point type property.
        /// </summary>
        public static Property Float => new Property("float");

        /// <summary>
        /// Begins a new 16 bit floating point type property.
        /// </summary>
        public static Property HalfFloat => new Property("half_float");

        /// <summary>
        /// Begins a new finite floating point number that is backed by a long, scaled by a fixed double scaling factor.
        /// </summary>
        public static Property ScaledFloat => new Property("scaled_float");

        /// <summary>
        /// Begins a new boolean type property.
        /// </summary>
        public static Property Boolean => new Property("boolean");

        /// <summary>
        /// Begins a new ip field that can index/store either IPv4 or IPv6 addresses.
        /// </summary>
        public static Property IPAddress => new Property("ip");

        /// <summary>
        /// Begins a new embedded object type property.
        /// </summary>
        public static Property Object => new Property("object");

        /// <summary>
        /// Begins a new nested object. The nested type is a specialized version of the object data type that 
        /// allows arrays of objects to be indexed in a way that they can be queried independently of each other.
        /// </summary>
        public static Property Nested => new Property("nested");

        /// <summary>
        /// Begins a new latitude/longitude pair type property supporting various geospatial operations.
        /// </summary>
        public static Property GeoPoint => new Property("geo_point");

        /// <summary>
        /// Begins a new geo-shape property. The geo_shape datatype facilitates the indexing of and searching with
        /// arbitrary geo-shapes such as rectangles and polygons. It should be used when either the data being
        /// indexed or the queries being executed contain shapes other than just points.
        /// </summary>
        public static Property GeoShape => new Property("geo_shape");

        /// <summary>
        /// Begins a new completion type property. The completion suggester provides auto-complete/search-as-you-type
        /// functionality. This is a navigational feature to guide users to relevant results as they are typing,
        /// improving search precision. It is not meant for spell correction or did-you-mean functionality like the
        /// term or phrase suggesters.
        /// </summary>
        public static Property Completion => new Property("completion");

        // Property attributes helpers

        /// <summary>
        /// The analyzer which should be used for analyzed string fields, both at 
        /// index-time and at search-time (unless overridden by the search_analyzer). Defaults to the 
        /// default index analyzer, or the standard analyzer.
        /// </summary>
        /// <param name="analyzer">The analyzer name.</param>
        public Property Analyzer(string analyzer) => AddTerm("analyzer", analyzer);

        /// <summary>
        /// The normalizer property of keyword fields is similar to analyzer
        /// except that it guarantees that the analysis chain produces a single token.
        /// </summary>
        /// <param name="normalizer">The normalizer name.</param>
        /// <returns></returns>
        public Property Normalizer(string normalizer) => AddTerm("normalizer", normalizer);

        /// <summary>
        /// Mapping field-level query time boosting. Accepts a floating point number, defaults to 1.0.
        /// </summary>
        /// <param name="boost">The boost value.</param>
        public Property Boost(float boost) => AddTerm("boost", boost);

        /// <summary>
        /// Multi-fields allow the same string value to be indexed in multiple ways for different 
        /// purposes, such as one field for search and a multi-field for sorting and aggregations, 
        /// or the same string value analyzed by different analyzers.
        /// </summary>
        /// <param name="fields">The fields object.</param>
        public Property Fields(object fields) => AddTerm("fields", fields);

        /// <summary>
        /// Copy to can be used to create custom "_all" fields. In other words, the values of multiple fields
        /// can be copied into a group field, which can then be queried as a single field.
        /// </summary>
        /// <param name="copyTo">The destination to copy this property to.</param>
        public Property CopyTo(string copyTo) => AddTerm("copyTo", copyTo);

        /// <summary>
        /// Specifies whether or not this property supports doc values,  which by
        /// default will be true.
        /// </summary>
        /// <param name="docValues">The doc values setting.</param>
        public Property DocValues(bool docValues) => AddTerm("docValues", docValues);

        /// <summary>
        /// The dynamic setting controls whether new fields can be added dynamically or not. It accepts
        /// three settings, true, false, or strict.
        /// </summary>
        /// <param name="dynamic">The dynamic setting.</param>
        public Property Dynamic(object dynamic) => AddTerm("dynamic", dynamic);

        /// <summary>
        /// The enabled setting, which can be applied only to the mapping type and to object fields, causes 
        /// Elasticsearch to skip parsing of the contents of the field entirely. The JSON can still be retrieved 
        /// from the _source field, but it is not searchable or stored in any other way.
        /// </summary>
        /// <param name="enabled">The enabled setting.</param>
        public Property Enabled(bool enabled) => AddTerm("enabled", enabled);

        /// <summary>
        /// Indicates whether to store fielddata for this property.
        /// </summary>
        /// <param name="fielddata">The fielddata setting.</param>
        public Property Fielddata(bool fielddata) => AddTerm("fielddata", fielddata);

        /// <summary>
        /// The maximum number length of strings to be indexed.
        /// </summary>
        /// <param name="ignoreAbove">The ignore above length.</param>
        public Property IgnoreAbove(int ignoreAbove) => AddTerm("ignoreAbove", ignoreAbove);

        /// <summary>
        /// Indicates that malformed data should be ignored for this field rather than causing the entire 
        /// document operation to error.
        /// </summary>
        /// <param name="ignoreMalformed">The ignore malformed setting.</param>
        public Property IgnoreMalformed(bool ignoreMalformed) => AddTerm("ignore_mal_formed", ignoreMalformed);

        /// <summary>
        /// An object containing the index options for this property, option properties include 'docs', 'freqs', 
        /// 'positions', and 'offsets'.
        /// </summary>
        /// <param name="indexOptions">The index options.</param>
        public Property IndexOptions(object indexOptions) => AddTerm("index_options", indexOptions);

        /// <summary>
        /// Indicates whether or not to index this property, this is true by default.
        /// </summary>
        /// <param name="index">This index setting.</param>
        public Property Index(bool index) => AddTerm("index", index);

        /// <summary>
        /// If enabled, two-term word combinations (shingles) are indexed into a separate field. This allows 
        /// exact phrase queries to run more efficiently, at the expense of a larger index. Note that this 
        /// works best when stopwords are not removed, as phrases containing stopwords will not use the subsidiary 
        /// field and will fall back to a standard phrase query. Default is false.
        /// </summary>
        /// <param name="indexPhrases">The index phrases setting.</param>
        public Property IndexPhrases(bool indexPhrases) => AddTerm("index_phrases", indexPhrases);

        /// <summary>
        /// Whether field-length should be taken into account when scoring queries. Default is false.
        /// </summary>
        /// <param name="norms">The norms setting.</param>
        public Property Norms(bool norms) => AddTerm("norms", norms);

        /// <summary>
        /// A null value cannot be indexed or searched. When a field is set to null, (or an empty array or an  array 
        /// of null values) it is treated as though that field has no values. The null_value parameter  allows you 
        /// to replace explicit null values with the specified value so that it can be indexed and searched.
        /// </summary>
        /// <param name="nullValue">The null value.</param>
        public Property NullValue(object nullValue) => AddTerm("null_value", nullValue);

        /// <summary>
        /// The number of fake term position which should be inserted between each element of an array of strings.
        /// Defaults to the position_increment_gap configured on the analyzer which defaults to 100. 100 was chosen
        /// because it prevents phrase queries with reasonably large slops (less than 100) from matching terms across
        /// field values.
        /// </summary>
        /// <param name="positionIncrementGap">The position increment gap.</param>
        public Property PositionIncrementGap(int positionIncrementGap) => AddTerm("position_increment_gap", positionIncrementGap);

        /// <summary>
        /// Defines the properties for a data types like 'object' or 'nested'
        /// </summary>
        /// <param name="properties">The properties.</param>
        public Property Properties(object properties) => AddTerm("properties", properties);

        /// <summary>
        /// The analyzer that should be used at search time on analyzed fields. Defaults to the analyzer setting.
        /// </summary>
        /// <param name="searchAnalyzer">The search analyzer.</param>
        public Property SearchAnalyzer(string searchAnalyzer) => AddTerm("search_analyzer", searchAnalyzer);

        /// <summary>
        /// Which scoring algorithm or similarity should be used. Defaults to BM25.
        /// </summary>
        /// <param name="similarity">The similarity.</param>
        public Property Similarity(string similarity) => AddTerm("similarity", similarity);

        /// <summary>
        /// Whether the field value should be stored and retrievable separately from the _source field.
        /// Default is false.
        /// </summary>
        /// <param name="store">The store setting.</param>
        public Property Store(bool store) => AddTerm("store", store);

        /// <summary>
        /// Indicates whether to apply eager global ordinals loading on this property.
        /// </summary>
        /// <param name="eagerGlobalOrdinals">The eager global ordinals setting.</param>
        public Property EagerGlobalOrdinals(bool eagerGlobalOrdinals) => AddTerm("eager_global_ordinals", eagerGlobalOrdinals);

        /// <summary>
        /// Term vectors contain information about the terms produced by the analysis process, including a list
        /// of terms,  the position (or order) of each term,  and the start and end character offsets mapping the
        /// term to its origin in the original string. Valid values include 'no', 'yes', 'with_positions', 
        /// 'with_offsets',  and 'with_positions_offsets'. Default is 'no'.
        /// </summary>
        /// <param name="termVector">The term vector setting.</param>
        public Property TermVector(string termVector) => AddTerm("term_vector", termVector);

        private Property AddTerm(string term, object value)
        {
            this[term] = value;
            return this;
        }
    }
}
