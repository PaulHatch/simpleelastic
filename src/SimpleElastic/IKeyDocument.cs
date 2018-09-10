using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Represents a document which supports key
    /// </summary>
    public interface IKeyDocument
    {
        /// <summary>
        /// Gets the key for this document.
        /// </summary>
        [JsonIgnore]
        object Key { get; }
    }

    /// <summary>
    /// Represents a document which 
    /// ID from a search query will be assigned to <see cref="Score"/>.
    /// </summary>
    public class KeyDocument<TKey> : IKeyDocument
    {
        /// <summary>
        /// Gets or sets the ID for this document.
        /// </summary>
        [JsonProperty("_id")]
        TKey Key { get; set; }

        /// <summary>
        /// Gets the ID for this document.
        /// </summary>
        object IKeyDocument.Key => Key;
    }
}
