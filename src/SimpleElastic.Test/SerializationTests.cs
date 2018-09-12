using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleElastic.Test
{
    public class SerializationTests
    {
        private class TestDocument
        {
            public FlatObject Test { get; set; }
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
            Assert.Contains("obj[0].a", doc.Test.Keys);
            Assert.Contains("obj[0].b", doc.Test.Keys);
            Assert.Contains("obj[0].c", doc.Test.Keys);
            Assert.Contains("obj[1].a", doc.Test.Keys);
            Assert.Contains("obj[1].b", doc.Test.Keys);
            Assert.Contains("obj[1].c[0]", doc.Test.Keys);
            Assert.Contains("obj[1].c[1]", doc.Test.Keys);
            Assert.Equal(1L, (long)doc.Test["obj[0].a"]);
            Assert.True((bool)doc.Test["obj[0].b"]);
            Assert.Null(doc.Test["obj[0].c"]);
            Assert.Equal(2L, (long)doc.Test["obj[1].a"]);
            Assert.False((bool)doc.Test["obj[1].b"]);
            Assert.Equal("test", doc.Test["obj[1].c[0]"]);
            Assert.Equal(3L, doc.Test["obj[1].c[1]"]);
        }

        [Fact]
        public void CanSerializeObjectQueryString()
        {
            var request = new { a = true };
            var result = QueryStringParser.GetQueryString(request);
            Assert.Equal("?a=true", result);
        }

        [Fact]
        public void CanSerializeMultipleObjectQueryString()
        {
            var request = new { a = true, b = 2, c = "test" };
            var result = QueryStringParser.GetQueryString(request);
            Assert.Equal("?a=true&b=2&c=test", result);
        }

        [Fact]
        public void EmptyObjectsCreateNoQueryString()
        {
            var request = new { };
            var result = QueryStringParser.GetQueryString(request);
            Assert.Equal(String.Empty, result);
        }

        [Fact]
        public void MapSerializationTest()
        {
            dynamic query = new Map {
                { "query", new { match_all = new {} }}
            };
            query.size = 10;
            query["from"] = 0;

            var json = JsonConvert.SerializeObject(query);

            // Generate the object we expect
            var expectedJson = JsonConvert.SerializeObject(new
            {
                query = new { match_all = new { } },
                size = 10,
                from = 0
            });

            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void EmptyBulkActionSerializesCorrectly()
        {
            var source = new BulkActionRequest(BulkActionType.Index) { Document = new { field = "value" } };
            var result = JsonConvert.SerializeObject(source);

            Assert.Equal("{\"index\":{}}\n{\"field\":\"value\"}\n", result);
        }

        [Fact]
        public void FullBulkActionSerializesCorrectly()
        {
            var source = new BulkActionRequest(BulkActionType.Index)
            {
                ID = "1",
                Index = "sample",
                Type = "_doc",
                Document = new { field = "value" }

            };
            var result = JsonConvert.SerializeObject(source);

            Assert.Equal("{\"index\":{\"_id\":\"1\",\"_index\":\"sample\",\"_type\":\"_doc\"}}\n{\"field\":\"value\"}\n", result);
        }

        [Fact]
        public void BulkCreateSerializesCorrectly()
        {
            var source = new BulkActionRequest(BulkActionType.Create) { Document = new { field = "value" } };
            var result = JsonConvert.SerializeObject(source);

            Assert.Equal("{\"create\":{}}\n{\"field\":\"value\"}\n", result);
        }

        [Fact]
        public void BulkUpdateSerializesCorrectly()
        {
            var source = new BulkActionRequest(BulkActionType.Update) { Document = new { doc = new { field = "value" } } };
            var result = JsonConvert.SerializeObject(source);

            Assert.Equal("{\"update\":{}}\n{\"doc\":{\"field\":\"value\"}}\n", result);
        }

        [Fact]
        public void BulkResponseDeserializedCorrectly()
        {
            var response = @"{""took"":5,""errors"":true,""items"":[{""index"":{""_index"":""sample"",""_type"":""sample"",""_id"":""1"",""_version"":9,""result"":""updated"",""_shards"":{""total"":2,""successful"":1,""failed"":0},""created"":false,""status"":200}},{""create"":{""_index"":""sample"",""_type"":""sample"",""_id"":""3"",""status"":409,""error"":{""type"":""version_conflict_engine_exception"",""reason"":""[sample][3]:version conflict, document already exists (current version [3])"",""index_uuid"":""vfwRxBKnRrakP99Gvb1BNQ"",""shard"":""4"",""index"":""sample""}}},{""create"":{""_index"":""sample"",""_type"":""sample"",""_id"":""4"",""status"":409,""error"":{""type"":""version_conflict_engine_exception"",""reason"":""[sample][4]:version conflict, document already exists (current version [1])"",""index_uuid"":""vfwRxBKnRrakP99Gvb1BNQ"",""shard"":""2"",""index"":""sample""}}},{""delete"":{""found"":false,""_index"":""sample"",""_type"":""sample"",""_id"":""5"",""_version"":3,""result"":""not_found"",""_shards"":{""total"":2,""successful"":1,""failed"":0},""status"":404}}]}";

            var result = JsonConvert.DeserializeObject<BulkActionResult>(response);

            Assert.True(result.HasErrors);
            Assert.Equal(5, result.Took.TotalMilliseconds);
            Assert.Equal(4, result.Items.Count());
        }

    }
}
