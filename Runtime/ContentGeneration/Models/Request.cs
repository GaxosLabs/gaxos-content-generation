using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Models
{
    public record Request
    {
        [JsonProperty("completedAt"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CompletedAt;
        [JsonProperty("createdAt"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CreatedAt;

        [JsonProperty("data")]
        public JObject Data;
        [JsonProperty("generator"), JsonConverter(typeof(GeneratorTypeConverter))]
        public Generator Generator;
        [JsonProperty("generatorParameters")]
        public JObject GeneratorParameters;
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("status"), JsonConverter(typeof(RequestStatusConverter))]
        public RequestStatus Status;

        [JsonProperty("failedDetails")]
        public FailedDetails FailedDetails;

        [JsonProperty("images")]
        public GeneratedImage[] Images;

        [JsonProperty("generationResult")] 
        public JObject GenerationResult;
    }
}