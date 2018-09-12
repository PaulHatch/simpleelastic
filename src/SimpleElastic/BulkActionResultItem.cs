using Newtonsoft.Json;
using SimpleElastic.Converters;

namespace SimpleElastic
{
    /// <summary>
    /// Represents the result of a bulk indexing request.
    /// </summary>
    [JsonConverter(typeof(BulkActionResultItemConverter))]
    public class BulkActionResultItem
    {
        /// <summary>
        /// Gets the document ID of the request.
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// Gets the index of the request.
        /// </summary>
        public string Index { get; internal set; }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Gets the document version.
        /// </summary>
        public long Version { get; internal set; }

        /// <summary>
        /// Gets the action taken.
        /// </summary>
        public BulkActionType Action { get; internal set; }

        /// <summary>
        /// Gets the result status code.
        /// </summary>
        public int StatusCode { get; internal set; }

        //public int TotalShards { get; internal set; }
        //public int SuccessfulShards { get; internal set; }
        //public int FailedShards { get; internal set; }

        /// <summary>
        /// Gets the error message, if any.
        /// </summary>
        public string Error { get; internal set; }
    }
}