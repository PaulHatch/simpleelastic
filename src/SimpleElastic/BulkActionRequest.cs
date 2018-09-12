using Newtonsoft.Json;
using SimpleElastic.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Represents a request for a document action within a bulk action request.
    /// </summary>
    [JsonConverter(typeof(BulkActionRequestConverter))]
    public sealed class BulkActionRequest
    {
        /// <summary>
        /// Represents a request for a document action within a bulk action request.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        public BulkActionRequest(BulkActionType action)
        {
            Action = action;
        }

        /// <summary>
        /// Gets the action type for this request.
        /// </summary>
        public BulkActionType Action { get; }

        /// <param name="documents">
        /// Gets or sets the document for this request, if  the action type for the request is 
        /// <see cref="BulkActionType.Index"/> or <see cref="BulkActionType.Create"/>, this is 
        /// the source document to index or create, if it is <see cref="BulkActionType.Update"/>
        /// this should be an update statement, and if it is <see cref="BulkActionType.Delete"/>
        /// this is either the ID or a document which implements <see cref="IKeyDocument{TKey}"/>.
        /// </param>
        public object Document { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the type, note that index types are deprecated in Elasticsearch 6.0.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the document ID for this request.
        /// </summary>
        public object ID { get; set; }
    }
}
