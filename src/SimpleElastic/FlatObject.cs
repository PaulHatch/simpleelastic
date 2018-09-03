using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleElastic
{
    [JsonConverter(typeof(FlatConverter))]
    public class FlatObject : Dictionary<string, object>
    {
    }
}
