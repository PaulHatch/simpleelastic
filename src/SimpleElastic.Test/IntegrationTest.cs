using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleElastic.Test
{
    [Collection("integration-test")]
    [TestCaseOrderer(IntegrationTestCaseOrderer.TypeName, IntegrationTestCaseOrderer.AssemblyName)]
    public class IntegrationTest
    {
        private static SimpleElasticClient _client = new SimpleElasticClient("http://localhost:9200");
        private static string _indexName = $"integration__test__{DateTime.UtcNow.Ticks}";

        private class TestIndex : KeyDocument<int>
        {
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }

        [Fact, Order(1)]
        public async Task CanCheckForNotExistingIndex()
        {
            var result = await _client.IndexExistsAsync(_indexName);

            Assert.False(result);
        }


        [Fact, Order(2)]
        public async Task CanCreateIndex()
        {
            var result = await _client.CreateIndexAsync(_indexName, new
            {
                settings = new
                {
                    number_of_shards = 2,
                    number_of_replicas = 0,
                    refresh_interval = "1s"
                },
                mappings = new
                {
                    _doc = new
                    {
                        properties = new Properties<TestIndex>
                        {
                            {
                                nameof(TestIndex.Field1),
                                Property.Text
                                    .Fields(new {
                                        raw = Property.Keyword
                                    })
                            }
                        }
                    }
                },
                aliases = new
                {
                    alias_1 = new { },
                    alias_2 = new
                    {
                        filter = Query.Term(nameof(TestIndex.Field1).ToName<TestIndex>(), "test")
                    }
                }
            });

            Assert.True(result.Acknowledged);
        }

        [Fact, Order(3)]
        public async Task CanGetIndex()
        {
            var result = await _client.GetIndexAsync(_indexName);

            Assert.NotNull(result[_indexName].Aliases["alias_2"]);
        }

        [Fact, Order(4)]
        public async Task CanBulkUpdateIndex()
        {
            var result = await _client.BulkActionAsync(_indexName, "_doc", BulkActionType.Create, new[] {
                new TestIndex { Key = 1, Field1 = "first", Field2 = "first" },
                new TestIndex { Key = 2, Field1 = "second", Field2 = "second" }
            });

            Assert.False(result.HasErrors);
            Assert.All(result.Items, i =>
            {
                Assert.Equal(BulkActionType.Create, i.Action);
                Assert.Equal(_indexName, i.Index);
                Assert.Equal("_doc", i.Type);
                Assert.Equal(201, i.StatusCode);
            });
        }

        [Fact, Order(5)]
        public async Task CanCheckForExistingIndex()
        {
            var result = await _client.IndexExistsAsync(_indexName);

            Assert.True(result);
        }
        
        [Fact, Order(100)]
        public async Task CanSearch()
        {
            // TODO: Remove once index initialization cases are finished
            await Task.Delay(1100);

            var result = await _client.SearchAsync<TestIndex>(_indexName, new { query = Query.MatchAll() });

            Assert.Equal(2, result.Hits.Count());
            Assert.Contains(result.Hits, i => i.Key == 1);
            Assert.Contains(result.Hits, i => i.Key == 2);
        }


        [Fact, Order(999999)]
        public async Task CanDeleteIndex()
        {
            var result = await _client.DeleteIndexAsync(_indexName);
            Assert.True(result.Acknowledged);
        }
    }
}
