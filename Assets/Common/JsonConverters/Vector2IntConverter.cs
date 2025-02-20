using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Common.JsonConverters
{
	public class Vector2IntConverter : JsonConverter
	{
		public override bool CanConvert(Type t)
		{
			return t == typeof(Vector2Int);
		}

		public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}

			var value = serializer.Deserialize<int[]>(reader);

			if (value.Length == 0)
			{
				return Vector2Int.zero;
			}
			else if (value.Length == 1)
			{
				return new Vector2Int(value[0], 0);
			}
			else
			{
				return new Vector2Int(value[0], value[1]);
			}
		}

		public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}

			var vector2 = (Vector2Int) untypedValue;
			serializer.Serialize(writer, new int[] { vector2[0], vector2[1] });
		}

		public static readonly Vector2IntConverter Singleton = new Vector2IntConverter();
	}
}
