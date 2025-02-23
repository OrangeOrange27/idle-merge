using System;
using System.Collections.Generic;
using Common.Serialization.Infrastructure;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using JsonSerializer = Common.Serialization.Implementation.JsonSerializer;

namespace Common.Serialization.Tests
{
	[TestFixture]
	public class SerializerTest
	{
		[SetUp]
		public void Setup()
		{
			_serializer = new JsonSerializer(new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
		}

		private readonly ForTestClass _forTestClass = new ForTestClass
		{
			Bool = true,
			String = "testing",
			Int = 8491724,
			Float = 32842.32423f
		};

		private readonly ForTestClassWithArray<int> _forTestClassWithArray = new ForTestClassWithArray<int>
		{
			Array = new[] { 1 }
		};

		private readonly ForTestClassWithDictionary<EnumForTest, string> _forTestClassWithDictionary =
			new ForTestClassWithDictionary<EnumForTest, string>
			{
				Dictionary = new Dictionary<EnumForTest, string>
				{
					{ EnumForTest.ONE, "test1" },
					{ EnumForTest.TWO, "test2" }
				}
			};

		private readonly ForTestStruct _forTestStruct = new ForTestStruct
		{
			Bool = true,
			String = "sdfsdf",
			Int = 9435,
			Float = -199.42f
		};

		private const EnumForTest EnumValueForTest = EnumForTest.TWO;

		private JsonSerializer _serializer;

		[Test]
		public void SerializerTest_SerializeDeserializeNull_ValuesAreEqual()
		{
			CheckSerialization<string>(null);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeBool_ValuesAreEqual()
		{
			CheckSerialization(true);
		}


		[Test]
		public void SerializerTest_SerializeDeserializeString_ValuesAreEqual()
		{
			CheckSerialization("test");
		}


		[Test]
		public void SerializerTest_SerializeDeserializeInt_ValuesAreEqual()
		{
			CheckSerialization(2345);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeFloat_ValuesAreEqual()
		{
			CheckSerialization(189.5345f);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeClass_ValuesAreEqual()
		{
			CheckSerialization(_forTestClass);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeStruct_ValuesAreEqual()
		{
			CheckSerialization(_forTestStruct);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeEnum_ValuesAreEqual()
		{
			CheckSerialization(EnumValueForTest);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeArray_ValuesAreEqual()
		{
			CheckSerialization(_forTestClassWithArray);
		}

		[Test]
		public void SerializerTest_SerializeDeserializeDictionary_ValuesAreEqual()
		{
			CheckSerialization(_forTestClassWithDictionary);
		}

		private void CheckSerialization<T>(T value)
		{
			var serializer = _serializer;
			serializer.Should().NotBeNull();

			var serializedStr = serializer.Serialize(value);
			var newValue = serializer.Deserialize<T>(serializedStr);
			newValue.Should().Be(value);
		}

		private class ForTestClass
		{
			public bool Bool;
			public float Float;
			public int Int;
			public string String;

			public override bool Equals(object obj)
			{
				if (obj != null && obj is ForTestClass forTestClass) return Equals(forTestClass);

				return base.Equals(obj);
			}

			protected bool Equals(ForTestClass other)
			{
				return Bool == other.Bool && String == other.String && Int == other.Int && Float.Equals(other.Float);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = Bool.GetHashCode();
					hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ Int;
					hashCode = (hashCode * 397) ^ Float.GetHashCode();
					return hashCode;
				}
			}
		}

		private class ForTestClassWithArray<T>
		{
			public T[] Array;

			public override bool Equals(object obj)
			{
				if (obj != null && obj is ForTestClassWithArray<T> otherClassWithArray) return Equals(otherClassWithArray);

				return base.Equals(obj);
			}

			protected bool Equals(ForTestClassWithArray<T> other)
			{
				var result = true;
				if (other.Array.Length == Array.Length)
				{
					for (var cIt = 0; cIt < Array.Length; cIt++)
						if (other.Array[cIt].Equals(Array[cIt]) == false)
						{
							result = false;
							break;
						}
				}
				else
				{
					result = false;
				}

				return result;
			}

			public override int GetHashCode()
			{
				return Array != null ? Array.GetHashCode() : 0;
			}
		}

		private class ForTestClassWithDictionary<TKey, TValue>
		{
			public Dictionary<TKey, TValue> Dictionary;

			public override bool Equals(object obj)
			{
				if (obj != null && obj is ForTestClassWithDictionary<TKey, TValue> otherClassWithDictionary) return Equals(otherClassWithDictionary);

				return base.Equals(obj);
			}

			protected bool Equals(ForTestClassWithDictionary<TKey, TValue> other)
			{
				var result = true;
				if (other.Dictionary.Count == Dictionary.Count)
					foreach (var pair in Dictionary)
					{
						if (other.Dictionary.ContainsKey(pair.Key) == false)
						{
							result = false;
							break;
						}

						if (other.Dictionary[pair.Key].Equals(pair.Value) == false)
						{
							result = false;
							break;
						}
					}
				else
					result = false;

				return result;
			}

			public override int GetHashCode()
			{
				return Dictionary != null ? Dictionary.GetHashCode() : 0;
			}
		}

		private struct ForTestStruct
		{
			public bool Bool;
			public string String;
			public int Int;
			public float Float;
		}

		public enum EnumForTest
		{
			ONE,
			TWO
		}
	}
}
