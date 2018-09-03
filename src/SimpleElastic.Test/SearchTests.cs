using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimpleElastic.Test
{
    public class SearchTests
    {
        // Result of aggregation query with a sub aggregation on "field1" and "field2" of a "sample" index
        private const string _response = @"{
  ""took"": 24,
  ""timed_out"": false,
  ""_shards"": {
    ""total"": 6,
    ""successful"": 6,
    ""skipped"": 0,
    ""failed"": 0
  },
  ""hits"": {
    ""total"": 3,
    ""max_score"": 0,
    ""hits"": []
    },
  ""aggregations"": {
    ""test"": {
      ""doc_count_error_upper_bound"": 0,
      ""sum_other_doc_count"": 0,
      ""buckets"": [
        {
          ""key"": ""one"",
          ""doc_count"": 2,
          ""test_inner"": {
            ""doc_count_error_upper_bound"": 0,
            ""sum_other_doc_count"": 0,
            ""buckets"": [
              {
                ""key"": ""two"",
                ""doc_count"": 1
              }
            ]
          }
        }
      ]
    }
  }
}";

        SimpleElasticClient sut = new SimpleElasticClient(new ClientOptions(new Uri("http://localhost:9200"))
        {
            HttpClient = new HttpMessageInvoker(new TestHandler(HttpStatusCode.OK, _response))
        });

        [Fact]
        public void DoesReturnAggregations()
        {
            var result = sut.SearchAsync<object>("test", new { }).Result;
            Assert.True(result.Aggregations.ContainsKey("test"));
        }

        [Fact]
        public void CanParseBucketCounts()
        {
            var result = sut.SearchAsync<object>("test", new { }).Result;
            Assert.Equal(2, result.Aggregations["test"].Buckets.Single().Count);
            Assert.Equal(1, result.Aggregations["test"].Buckets.Single().SubAggregations["test_inner"].Buckets.Single().Count);
        }

        [Fact]
        public void CanParseBucketKeys()
        {
            var result = sut.SearchAsync<object>("test", new { }).Result;
            Assert.Equal("one", result.Aggregations["test"].Buckets.Single().Key);
            Assert.Equal("two", result.Aggregations["test"].Buckets.Single().SubAggregations["test_inner"].Buckets.Single().Key);
        }
    }
}
