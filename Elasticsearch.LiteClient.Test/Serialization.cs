using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace Elasticsearch.LiteClient.Test
{
    public class Serialization
    {
        class TestDocument
        {
            [JsonConverter(typeof(FlatConverter))]
            public IDictionary<string,object> Test { get; set; }
        }


        [Fact]
        public void FlattenSerializerBasicTest()
        {
            var json = "{ \"Test\": { \"obj\": { \"a\": 1, \"b\": true, \"c\": null } } }";
            var doc = JsonConvert.DeserializeObject<TestDocument>(json);

            Assert.Equal(3, doc.Test.Count);
            Assert.Contains("obj.a", doc.Test.Keys);
            Assert.Contains("obj.b", doc.Test.Keys);
            Assert.Contains("obj.c", doc.Test.Keys);
            Assert.Equal(1L, (long)doc.Test["obj.a"]);
            Assert.True((bool)doc.Test["obj.b"]);
            Assert.Null(doc.Test["obj.c"]);
        }

        [Fact]
        public void FlattenSerializerArrayTest()
        {
            var json = "{ \"Test\": { \"obj\": [{ \"a\": 1, \"b\": true, \"c\": null }, { \"a\": 2, \"b\": false, \"c\": [\"test\", 3] }] } }";
            var doc = JsonConvert.DeserializeObject<TestDocument>(json);

            Assert.Equal(7, doc.Test.Count);
            Assert.Contains("obj.0.a", doc.Test.Keys);
            Assert.Contains("obj.0.b", doc.Test.Keys);
            Assert.Contains("obj.0.c", doc.Test.Keys);
            Assert.Contains("obj.1.a", doc.Test.Keys);
            Assert.Contains("obj.1.b", doc.Test.Keys);
            Assert.Contains("obj.1.c.0", doc.Test.Keys);
            Assert.Contains("obj.1.c.1", doc.Test.Keys);
            Assert.Equal(1L, (long)doc.Test["obj.0.a"]);
            Assert.True((bool)doc.Test["obj.0.b"]);
            Assert.Null(doc.Test["obj.0.c"]);
            Assert.Equal(2L, (long)doc.Test["obj.1.a"]);
            Assert.False((bool)doc.Test["obj.1.b"]);
            Assert.Equal("test", doc.Test["obj.1.c.0"]);
            Assert.Equal(3L, doc.Test["obj.1.c.1"]);
        }
    }
}
