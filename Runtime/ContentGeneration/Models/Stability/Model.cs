using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum Model
    {
        Sd3Large, Sd3LargeTurbo, Sd3Medium
    }

    internal class ModelConverter : EnumJsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            writer.WriteValue(CamelCaseToDashes(value.ToString()));
        }

        protected override string AdaptString(string str)
        {
            return DashesToCamelCase(str);
        }
    }
}