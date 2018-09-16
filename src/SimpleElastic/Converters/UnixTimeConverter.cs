using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace SimpleElastic.Converters
{
    internal class UnixTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(DateTimeOffset);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTimeOffset)value).ToUnixTimeMilliseconds());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!Int64.TryParse(reader.Value?.ToString(), out long time))
            {
                return default(DateTimeOffset);
            }

            return DateTimeOffset.FromUnixTimeMilliseconds(time);
        }
    }
}