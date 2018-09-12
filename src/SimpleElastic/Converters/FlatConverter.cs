using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleElastic.Converters
{
    internal class FlatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new FlatObject();
            var startDepth = reader.Depth;
            var pathLength = reader.Path.Length + 1;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        result.Add(reader.Path.Substring(pathLength), reader.Value);
                        break;
                    case JsonToken.EndObject:
                        if (reader.Depth == startDepth)
                        {
                            return result;
                        }
                        break;
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
