using System;
using System.Collections.Generic;
using System.Text;
namespace SimpleElastic
{
    /// <summary>
    /// Mapping helper
    /// </summary>
    public static class Mapping
    {
        /// <summary>
        /// Creates a new property mapping definition.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="normalizer">The normalizer property of keyword fields is similar to analyzer
        /// except that it guarantees that the analysis chain produces a single token.</param>
        /// <param name="boost">Individual fields can be boosted automatically — count more towards
        /// the relevance score — at query time,  with the boost parameter.</param>
        /// <param name="fields">Additional fields for this property.</param>
        /// <param name="coerce">Coercion attempts to clean up dirty values to fit the datatype of a
        /// field. For instance strings will be coerced to numbers and floating points will be truncated
        /// for integer values.</param>
        /// <param name="copyTo">The copyTo parameter allows you to create custom "_all" fields. In other
        /// words,  the values of multiple fields can be copied into a group field,  which can then be
        /// queried as a single field.</param>
        /// <param name="docValues">Specifies whether or not this property supports doc values,  which by
        /// default will be true.</param>
        /// <param name="dynamic">The dynamic setting controls whether new fields can be added dynamically
        /// or not. It accepts three settings,  true,  false,  or strict.</param>
        /// <param name="enabled">The enabled setting,  which can be applied only to the mapping type and
        /// to object fields,  causes Elasticsearch to skip parsing of the contents of the field entirely.
        /// The JSON can still be retrieved from the _source field,  but it is not searchable or stored in
        /// any other way</param>
        /// <param name="fielddata">Indicates whether to store fielddata for this property.</param>
        /// <param name="format">A custom date format.</param>
        /// <param name="ignoreAbove">The maximum number length of strings to be indexed.</param>
        /// <param name="ignoreMalformed">Indicates that malformed data should be ignored for this field
        /// rather than causing the entire document operation to error.</param>
        /// <param name="indexOptions">An object containing the index options for this property,  option
        /// properties include 'docs',  'freqs',  'positions',  and 'offsets'.</param>
        /// <param name="index">Indicates whether or not to index this property,  is is true by default.</param>
        /// <param name="norms">The norms.</param>
        /// <param name="nullValue">The null value.</param>
        /// <param name="positionIncrementGap">The position increment gap.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="searchAnalyzer">The search analyzer.</param>
        /// <param name="similarity">The similarity.</param>
        /// <param name="store">The store.</param>
        /// <param name="eagerGlobalOrdinals">Indicates whether to apply eager global ordinals loading on
        /// this property.</param>
        /// <param name="termVector">Term vectors contain information about the terms produced by the analysis process,  including
        /// a list of terms,  the position (or order) of each term,  and the start and end character offsets
        /// mapping the term to its origin in the original string. Valid values include 'no',  'yes',
        /// 'with_positions',  'with_offsets',  and 'with_positions_offsets'. Default is 'no'.</param>
        /// <returns>
        /// An object representing a property definition.
        /// </returns>
        public static object Property(
            string type, 
            string analyzer = null, 
            string normalizer = null, 
            float? boost = null, 
            object fields = null, 
            bool? coerce = null, 
            string copyTo = null, 
            bool? docValues = null, 
            object dynamic = null, 
            bool? enabled = null, 
            bool? fielddata = null, 
            string format = null, 
            int? ignoreAbove = null, 
            bool? ignoreMalformed = null, 
            object indexOptions = null, 
            bool? index = null, 
            bool? norms = null, 
            object nullValue = null, 
            int? positionIncrementGap = null, 
            object properties = null, 
            string searchAnalyzer = null, 
            string similarity = null, 
            bool? store = null, 
            bool? eagerGlobalOrdinals = null, 
            string termVector = null)
        {
            return new Map
            {
                { "type", type }, 
                { "analyzer", analyzer, analyzer != null }, 
                { "normalizer", normalizer, normalizer != null }, 
                { "boost", boost, boost != null }, 
                { "coerce", coerce, coerce != null }, 
                { "copy_to", copyTo, copyTo != null }, 
                { "doc_values", docValues, docValues != null }, 
                { "dynamic", dynamic, dynamic != null }, 
                { "enabled", enabled, enabled != null }, 
                { "fielddata", fielddata, fielddata != null }, 
                { "eager_global_ordinals", eagerGlobalOrdinals, eagerGlobalOrdinals != null }, 
                { "format", format, format != null }, 
                { "ignore_above", ignoreAbove, ignoreAbove != null }, 
                { "ignore_malformed", ignoreMalformed, ignoreMalformed != null }, 
                { "index_options", indexOptions, indexOptions != null }, 
                { "index", index, index != null }, 
                { "fields", fields, fields != null }, 
                { "norms", norms, norms != null }, 
                { "null_value", nullValue, nullValue != null }, 
                { "position_increment_gap", positionIncrementGap, positionIncrementGap != null }, 
                { "properties", properties, properties != null }, 
                { "search_analyzer", searchAnalyzer, searchAnalyzer != null }, 
                { "similarity", similarity, similarity != null }, 
                { "store", store, store != null }, 
                { "term_vector", termVector, termVector != null }
             };
         }
     }
 }
