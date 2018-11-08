using Newtonsoft.Json;
using System;

namespace SimpleElastic
{
    /// <summary>
    /// Represents a document for which the document ID from a search query
    /// will be assigned to <see cref="Key"/>.
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
