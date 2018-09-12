using Newtonsoft.Json;
using System;

namespace SimpleElastic.Converters
{
    internal class BulkActionResultItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new BulkActionResultItem();
            var startDepth = reader.Depth;

            reader.Read();
            if (Enum.TryParse<BulkActionType>(reader.Value as string, true, out BulkActionType actionType))
            {
                result.Action = actionType;
            }
            else
            {
                // TODO: Throw exception? The action may have succeeded we've just received an invalid response.
                return null;
            }

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.PropertyName:
                        switch (reader.Value as string)
                        {
                            case "_id":
                                result.ID = reader.ReadAsString();
                                break;
                            case "_index":
                                result.Index = reader.ReadAsString();
                                break;
                            case "_type":
                                result.Type = reader.ReadAsString();
                                break;
                            case "_version":
                                result.Version = reader.ReadAsInt32().Value;
                                break;
                            case "status":
                                result.StatusCode = reader.ReadAsInt32().Value;
                                break;
                            case "error":
                                var depth = reader.Depth;
                                while (reader.Read() && reader.Depth >= depth)
                                {
                                    if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == "message")
                                    {
                                        // TODO: this will override errors with nested errors, but it is good enough for now
                                        result.Error = reader.ReadAsString();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
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