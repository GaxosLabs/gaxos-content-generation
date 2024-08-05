using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public record Stats
    {
        public record StatsFloat
        {
            [JsonProperty("used")]
            public float Used;
            [JsonProperty("total")]
            public float Total;

            public override string ToString()
            {
                return $"{Total - Used:0.##} / {Total:0.##}";
            }
        }
        public record StatsULong
        {
            [JsonProperty("used")]
            public ulong Used;
            [JsonProperty("total")]
            public ulong Total;

            public override string ToString()
            {
                return $"{Total - Used} / {Total}";
            }
        }

        [JsonProperty("credits")] public StatsFloat Credits;
        [JsonProperty("storage")] public StatsULong Storage;
        [JsonProperty("requests")] public StatsULong Requests;
    }
}