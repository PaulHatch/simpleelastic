using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SimpleElastic.Converters
{
    /// <summary>
    /// Converter for Map type, serializes the wrapped values of the map.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    internal class MapConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var values = serializer.Deserialize<IDictionary<string, object>>(reader);
            return new Map(values);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as Map).Values);
        }
    }
}