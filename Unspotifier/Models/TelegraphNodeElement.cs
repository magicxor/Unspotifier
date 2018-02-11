using System.Collections.Generic;
using Newtonsoft.Json;

namespace Unspotifier.Models
{
    public class TelegraphNodeElement
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("attrs")]
        public IDictionary<string, string> Attributes { get; set; }

        [JsonProperty("children")]
        public IList<TelegraphNodeElement> Children { get; set; }
    }
}
