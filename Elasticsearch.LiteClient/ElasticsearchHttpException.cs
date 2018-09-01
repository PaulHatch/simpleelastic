using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Elasticsearch.LiteClient
{
    public class ElasticsearchHttpException : HttpRequestException
    {
        public ElasticsearchHttpException()
        {
        }
        
        public ElasticsearchHttpException(string message) : base(message)
        {
        }

        public ElasticsearchHttpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}