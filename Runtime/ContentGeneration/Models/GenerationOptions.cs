using ContentGeneration.Models.Stability;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models
{
    public record GenerationOptions
    {
        public const GenerationOptions None = null;

        [JsonProperty("transparentColor"), JsonConverter(typeof(ColorConverter))]
        public Color? TransparentColor;

        [JsonProperty("transparentColorReplaceDelta")]
        public float TransparentColorReplaceDelta = 20;

        [JsonProperty("improvePrompt")]
        public bool ImprovePrompt;
    }
}