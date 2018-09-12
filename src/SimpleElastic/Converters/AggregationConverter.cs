using Newtonsoft.Json;
using SimpleElastic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic.Converters
{
    /// <summary>
    /// Internal deserializer to map results to our aggregation result type.
    /// </summary>
    internal class AggregationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new Aggregation();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var name = reader.Value as string;
                        reader.Read();
                        switch (name)
                        {
                            case "doc_count_error_upper_bound": 
                                result.DocumentCountErrorUpperBound = serializer.Deserialize<long>(reader);
                                break;
                            case "sum_other_doc_count":
                                result.SumOtherDocumentCount = serializer.Deserialize<long>(reader);
                                break;
                            case "buckets":
                                if (reader.TokenType == JsonToken.StartArray)
                                {
                                    var aggs = new List<AggregationBucket>();
                                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                                    {
                                        aggs.Add(serializer.Deserialize<AggregationBucket>(reader));
                                    }
                                    result.Buckets = aggs;
                                }
                                break;
                            default:
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    var agg = serializer.Deserialize<Aggregation>(reader);
                                    result.AddAggregation(name, agg);
                                }
                                break;
                        }
                        break;
                    case JsonToken.EndObject:
                        return result;
                    default:
                        break;
                }
            }
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
