using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Models
{
    public record Request
    {
        [JsonProperty("completed_at"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CompletedAt;
        [JsonProperty("created_at"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CreatedAt;

        [JsonProperty("data")]
        public JObject Data;
        [JsonProperty("generator"), JsonConverter(typeof(GeneratorTypeConverter))]
        public Generator Generator;
        [JsonProperty("generator_parameters")]
        public JObject GeneratorParameters;
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("status"), JsonConverter(typeof(RequestStatusConverter))]
        public RequestStatus Status;

        [JsonProperty("failed_details")]
        public FailedDetails FailedDetails;

        [JsonProperty("assets")]
        public GeneratedAssets[] Assets;

        [JsonProperty("generation_result")] 
        public JObject GenerationResult;
    }
}