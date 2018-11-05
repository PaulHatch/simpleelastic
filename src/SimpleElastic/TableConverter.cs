using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    internal class TableConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var result = new Table();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.EndObject:
                        return result;
                    case JsonToken.PropertyName:
                        var propertyName = reader.Value.ToString();
                        reader.Read();
                        result.Add(propertyName, serializer.Deserialize(reader, typeof(object)));
                        break;
                    default:
                        throw new JsonSerializationException($"Error while deserializing properties for table, unexpected token {reader.TokenType}.");
                }
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Table table)
            {
                writer.WriteStartObject();
                foreach (var item in table)
                {
                    writer.WritePropertyName(item.Key);
                    serializer.Serialize(writer, item.Value);
                }
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteNull();
            }
        }

    }
}
