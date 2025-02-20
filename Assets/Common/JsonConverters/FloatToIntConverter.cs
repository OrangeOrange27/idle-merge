using System;
using Newtonsoft.Json;

namespace Common.JsonConverters
{
    public class FloatToIntConverter : JsonConverter<int>
    {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Float)
            {
                // Round and convert to int
                double doubleValue = Convert.ToDouble(reader.Value);
                return (int)Math.Round(doubleValue);
            }

            return Convert.ToInt32(reader.Value);
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}