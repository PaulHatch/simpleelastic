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
        object Key { get; set; }
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
        [JsonIgnore]
        public TKey Key { get; set; }

        /// <summary>
        /// Gets the ID for this document.
        /// </summary>
        object IKeyDocument.Key
        {
            get { return Key; }
            set
            {
                switch (value)
                {
                    case TKey key:
                        Key = key;
                        break;
                    default:
                        Key = (TKey)Convert.ChangeType(value, typeof(TKey));
                        break;
                }
            }
        }
    }
}
