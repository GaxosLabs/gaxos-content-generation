using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Comfy
{
    public enum Model
    {
        Comfy2, Comfy3 
    }
    internal class ModelConverter : EnumJsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            var str = value switch
            {
                Model.Comfy2 => "dall-e-2",
                Model.Comfy3 => "dall-e-3",
                _ => value.ToString().ToUpperInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "");
        }
    }
}