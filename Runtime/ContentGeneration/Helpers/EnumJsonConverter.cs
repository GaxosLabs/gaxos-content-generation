using System;
using Newtonsoft.Json;

namespace ContentGeneration.Helpers
{
    internal class EnumJsonConverter<T> : JsonConverter<T> where T : struct, IConvertible
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public sealed override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var valueString = AdaptString((string)reader.Value!);
            foreach (var value in Enum.GetValues(typeof(T)))
            {
                if (valueString == value.ToString().ToLowerInvariant())
                {
                    return (T)value;
                }
            }

            throw new ArgumentOutOfRangeException(valueString);
        }

        protected virtual string AdaptString(string str)
        {
            return str.ToLowerInvariant();
        }

        protected static string CamelCaseToDashes(string str)
        {
            var lowerStr = str.ToLowerInvariant();
            var ret = lowerStr[0].ToString();

            for (var i = 1; i < str.Length; i++)
            {
                if (char.IsUpper(str[i]))
                {
                    ret += '-';
                }

                ret += lowerStr[i];
            }

            return ret;
        }

        protected static string DashesToCamelCase(string str)
        {
            var ret = "";

            var nextIsUpperCase = true;
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] == '-')
                {
                    nextIsUpperCase = true;
                }
                else
                {
                    var nextChar = str[i];
                    if (nextIsUpperCase)
                    {
                        nextChar = char.ToUpperInvariant(nextChar);
                    }

                    ret += nextChar;
                    
                    nextIsUpperCase = false;
                }
            }

            return ret;
        }
    }
}