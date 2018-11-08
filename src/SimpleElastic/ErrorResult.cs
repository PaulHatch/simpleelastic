using Newtonsoft.Json;
using System.Collections.Generic;

namespace SimpleElastic
{
    /// <summary>
    /// Represents an error from an Elasticsearch request.
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// Gets or sets the error object.
        /// </summary>
        [JsonProperty("error")]
        public RootErrorDetail Error { get; set; }

        /// <summary>
        /// Gets or sets the response status.
        /// </summary>
        [JsonProperty("status")]
        public int Status { get; set; }

        /// <summary>
        /// Root details element of an error result.
        /// </summary>
        public class RootErrorDetail : ErrorDetail
        {
            /// <summary>
            /// Gets or sets the root caused clause if provided.
            /// </summary>
            [JsonProperty("root_cause")]
            public IEnumerable<ErrorDetail> RootCause { get; set; }

            /// <summary>
            /// Gets or sets the caused by clause if provided.
            /// </summary>
            [JsonProperty("caused_by")]
            public ErrorDetail CausedBy { get; set; }
        }

        /// <summary>
        /// Details element of an error result.
        /// </summary>
        public class ErrorDetail
        {
            /// <summary>
            /// Gets or sets the error type.
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the reason for the error.
            /// </summary>
            [JsonProperty("reason")]
            public string Reason { get; set; }

            /// <summary>
            /// Gets or sets the stack trace if stack traces were enabled.
            /// </summary>
            [JsonProperty("stack_trace")]
            public string StackTrace { get; set; }

            /// <summary>
            /// Returns a <see cref="System.String" /> that represents this instance.
            /// </summary>
            /// <returns>
            /// A <see cref="System.String" /> that represents this instance.
            /// </returns>
            public override string ToString()
            {
                return $"{Type}: {Reason}";
            }
        }
    }
}