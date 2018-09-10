using System;
using Newtonsoft.Json;

namespace SimpleElastic
{
    internal class BulkActionRequestConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var action = value as BulkActionRequest;
            writer.WriteStartObject();
            writer.WritePropertyName(action.Action.ToString().ToLower());
            writer.WriteStartObject();
            if (action.ID != null)
            {
                writer.WritePropertyName("_id");
                writer.WriteValue(action.ID);
            }
            if (!String.IsNullOrEmpty(action.Index))
            {
                writer.WritePropertyName("_index");
                writer.WriteValue(action.Index);
            }
            if (!String.IsNullOrEmpty(action.Type))
            {
                writer.WritePropertyName("_type");
                writer.WriteValue(action.Type);
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
            writer.WriteRaw("\n");
            switch (action.Action)
            {
                case BulkActionType.Index:
                case BulkActionType.Create:
                case BulkActionType.Update:
                    serializer.Serialize(writer, action.Document);
                    writer.WriteRaw("\n");
                    break;
                case BulkActionType.Delete:
                default:
                    break;
            }
            
        }
    }
}