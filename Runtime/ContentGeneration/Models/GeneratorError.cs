using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public record GeneratorError
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("error")]
        public string Error;
    }
}