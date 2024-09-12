using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum Mode
    {
        TextToImage,
        ImageToImage
    }

    internal class ModeConverter : EnumJsonConverter<Mode>
    {
        public override void WriteJson(JsonWriter writer, Mode value, JsonSerializer serializer)
        {
            writer.WriteValue(CamelCaseToDashes(value.ToString()));
        }

        protected override string AdaptString(string str)
        {
            return DashesToCamelCase(str);
        }
    }
}