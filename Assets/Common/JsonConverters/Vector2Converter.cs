using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Common.JsonConverters
{
	public class Vector2Converter : JsonConverter
	{
		public override bool CanConvert(Type t)
		{
			return t == typeof(Vector2);
		}

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			var value = serializer.Deserialize<float[]>(reader);

			if (value.Length == 0)
			{
				return Vector2.zero;
			}
			else if (value.Length == 1)
			{
				return new Vector2(value[0], 0);
			}
			else
			{
				return new Vector2(value[0], value[1]);
			}
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}

			var vector2 = (Vector2) untypedValue;
			serializer.Serialize(writer, new float[] { vector2[0], vector2[1] });
		}

		public static readonly Vector2Converter Singleton = new Vector2Converter();
	}
}