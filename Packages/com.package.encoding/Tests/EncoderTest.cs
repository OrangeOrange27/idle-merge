using System;
using System.Collections.Generic;
using Common.Encoding.Implementation;
using Common.Encoding.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace Common.Encoding.Tests
{
	[TestFixture]
	[TestFixtureSource(nameof(Encoders))]
	public class EncoderTest
	{
		private readonly IEncoder _encoder;

		public EncoderTest(IEncoder encoder)
		{
			_encoder = encoder;
		}

		public static IEnumerable<IEncoder> Encoders
		{
			get
			{
				return new IEncoder[]
				{
					//todo: HTMLEncoder has failed tests if input string has symbols and german letters
					// new HtmlEncoder(),
					new GenericEncoder(),
					new CryptEncoder()
				};
			}
		}

		[Test]
		public void Encoder_EncodeDecodeNumbers_ValuesAreTheSame()
		{
			var inputStr = "1234567890";

			CheckEncodeDecodeValues(inputStr);
		}

		[Test]
		public void Encoder_EncodeDecodeLetters_ValuesAreTheSame()
		{
			var inputStr = @"äöüßqwertyuiopasdfghjklzxcvbnm";

			CheckEncodeDecodeValues(inputStr);
		}

		[Test]
		public void Encoder_EncodeDecodeSymbols_ValuesAreTheSame()
		{
			var inputStr = @"»«£€{}.,!|@#$%^&*()<>/?\;:'`~[]\±§+-_= ";

			CheckEncodeDecodeValues(inputStr);
		}

		private void CheckEncodeDecodeValues(string inputStr)
		{
			var encoder = _encoder;

			var encodedStr = encoder.Encode(inputStr);
			var decodedStr = encoder.Decode(encodedStr);

			decodedStr.Should().Be(inputStr);
		}
	}
}