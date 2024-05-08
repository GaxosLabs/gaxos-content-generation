using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Models
{
    public record PublishedImage
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("url")]
        public string URL;

        [JsonProperty("data")]
        public JObject Data;
        
        [JsonProperty("createdAt"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CreatedAt;

        [JsonProperty("request")]
        public Request Request;
    }
}