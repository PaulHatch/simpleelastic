using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic.Converters
{
    /// <summary>
    /// Internal deserializer to map results to our aggregation bucket result type.
    /// </summary>
    internal class AggregationBucketConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new AggregationBucket();
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        var name = reader.Value as string;
                        reader.Read();
                        switch (name)
                        {
                            case "key":
                                result.Key = serializer.Deserialize<string>(reader);
                                break;
                            case "doc_count":
                                result.Count = serializer.Deserialize<long>(reader);
                                break;
                            case "hits":
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    result.Hits = serializer.Deserialize<IEnumerable<FlatObject>>(reader);
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
