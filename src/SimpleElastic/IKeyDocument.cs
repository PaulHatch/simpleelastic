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
}
