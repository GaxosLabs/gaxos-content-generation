using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public record GeneratedAssets
    {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("url")]
        public string URL;
        [JsonProperty("will_be_public")]
        public bool WillBePublic;
    }
}