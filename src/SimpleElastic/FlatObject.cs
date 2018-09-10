using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    /// <summary>
    /// Represents an inner or top hits result. Each field is stored as a key-value pair
    /// where the key is a string path of the field and the value is the property value
    /// of the result. This class inherits from 
    /// <see cref="System.Collections.Generic.Dictionary{System.String, System.Object}" />.
    /// </summary>
    [JsonConverter(typeof(FlatConverter))]
    public class FlatObject : Dictionary<string, object>
    {
    }
}
