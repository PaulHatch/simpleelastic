using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elasticsearch.LiteClient
{
    public class FlatConverter : JsonConverter
    {
        private class ArrayInfo { public int Depth; public int Index; }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new Dictionary<string, object>();
            var path = new Stack<string>();
            var array = new Stack<ArrayInfo>();
            var startDepth = reader.Depth;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartArray:
                        path.Push("0");
                        array.Push(new ArrayInfo { Depth = reader.Depth });
                        break;
                    case JsonToken.EndArray:
                        array.Pop();
                        break;
                    case JsonToken.PropertyName:
                        path.Push(reader.Value.ToString());
                        break;
                    case JsonToken.Integer:
                    case JsonToken.Float:
                    case JsonToken.String:
                    case JsonToken.Boolean:
                    case JsonToken.Null:
                    case JsonToken.Date:
                    case JsonToken.Bytes:
                        result.Add(String.Join(".", path.Reverse()), reader.Value);
                        path.Pop();
                        if (array.Any() && array.Peek().Depth == reader.Depth - 1)
                        {
                            var arrayInfo = array.Peek();
                            arrayInfo.Index++;
                            path.Push(arrayInfo.Index.ToString());
                        }
                        break;
                    case JsonToken.EndObject:
                        if (reader.Depth == startDepth)
                        {
                            return result;
                        }
                        path.Pop();
                        if (array.Any() && array.Peek().Depth == reader.Depth - 1)
                        {
                            var arrayInfo = array.Peek();
                            arrayInfo.Index++;
                            path.Push(arrayInfo.Index.ToString());
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
