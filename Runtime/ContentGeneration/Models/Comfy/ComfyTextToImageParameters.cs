using Newtonsoft.Json;

namespace ContentGeneration.Models.Comfy
{
    public record ComfyTextToImageParameters : ComfyParameters
    {
        [JsonProperty("width")] public uint Width = 512;
        [JsonProperty("height")] public uint Height = 512;
    }
}