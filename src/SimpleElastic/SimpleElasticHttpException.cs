using System;
using System.Net.Http;
using System.Runtime.Serialization;

namespace SimpleElastic
{
    /// <summary>
    /// Represents an HTTP exception (inheirits from <see cref="System.Net.Http.HttpRequestException"/>)
    /// during a request to elasticsearch.
    /// </summary>
    /// <seealso cref="System.Net.Http.HttpRequestException" />
    public class SimpleElasticHttpException : HttpRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticHttpException"/> class.
        /// </summary>
        public SimpleElasticHttpException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticHttpException"/> class.
        /// </summary>
        /// <param name="message">A message that describes the current exception.</param>
        public SimpleElasticHttpException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleElasticHttpException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public SimpleElasticHttpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}