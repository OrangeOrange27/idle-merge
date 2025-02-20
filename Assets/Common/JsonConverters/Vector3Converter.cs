using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Common.JsonConverters
{
	public class Vector3Converter : JsonConverter
	{
		public override bool CanConvert(Type t)
		{
			return t == typeof(Vector3);
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
				return Vector3.zero;
			}
			else if (value.Length == 1)
			{
				return new Vector3(value[0], 0);
			}
			else if (value.Length == 2)
			{
				return new Vector3(value[0], value[1]);
			}
			else
			{
				return new Vector3(value[0], value[1], value[2]);
			}
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}

			var vector3 = (Vector3) untypedValue;
			serializer.Serialize(writer, new float[] { vector3[0], vector3[1], vector3[2] });
		}

		public static readonly Vector3Converter Singleton = new Vector3Converter();
	}
}