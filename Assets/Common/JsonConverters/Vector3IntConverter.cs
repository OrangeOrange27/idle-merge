using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Common.JsonConverters
{
	public class Vector3IntConverter : JsonConverter
	{
		public static readonly Vector3IntConverter Singleton = new Vector3IntConverter();

		public override bool CanConvert(Type t)
		{
			return t == typeof(Vector3Int);
		}

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;

			var value = serializer.Deserialize<int[]>(reader);

			if (value.Length == 0)
				return Vector3Int.zero;
			if (value.Length == 1)
				return new Vector3Int(value[0], 0);
			if (value.Length == 2)
				return new Vector3Int(value[0], value[1]);
			return new Vector3Int(value[0], value[1], value[2]);
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}

			var vector3 = (Vector3Int)untypedValue;
			serializer.Serialize(writer, new float[] { vector3[0], vector3[1], vector3[2] });
		}
	}
}
