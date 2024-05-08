using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public record FailedDetails
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("error")]
        public string Error;
    }
}