using System.Collections.Generic;
using System.Net;
using Common.Encoding.Implementation;
using Common.Encoding.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace Common.Encoding.Tests
{
	[TestFixture]
	[TestFixtureSource(nameof(Encoders))]
	public class StoredDataConverterTest
	{
		private readonly IEncoder _encoder;

		private readonly string _encodedText;

		private readonly string _decodedText;

		public StoredDataConverterTest(IEncoder encoder)
		{
			_encoder = encoder;

			_encodedText =
				@"{&#10;&#9;&quot;IsMusicEnabled&quot;: true,&#10;&#9;&quot;IsSoundEnabled&quot;: false,&#10;&#9;&quot;IsVoiceEnabled&quot;: true,&#10;&#9;&quot;IsNotificationEnabled&quot;: true,&#10;&#9;&quot;AppVersion&quot;: &quot;1.76.4&quot;,&#10;&#9;&quot;UUID&quot;: &quot;FD240B08-DC7C-401F-93AD-5C2BA054F387&quot;,&#10;&#9;&quot;DeviceName&quot;: &quot;OnePlus5&quot;,&#10;&#9;&quot;ModelName&quot;: &quot;ONEPLUS A5000&quot;,&#10;&#9;&quot;IsSyncEnabled&quot;: true,&#10;&#9;&quot;FacebookInfoSent&quot;: false,&#10;&#9;&quot;FacebookCheckAppRequest&quot;: 5,&#10;&#9;&quot;TriggerMetaDataSync&quot;: false,&#10;&#9;&quot;TriggerSendLogDataToPsf&quot;: false,&#10;&#9;&quot;AnalyticsFirebaseDisabled&quot;: false,&#10;&#9;&quot;AnalyticsAppsFlyerDisabled&quot;: false,&#10;&#9;&quot;LastBoottime&quot;: 1605622410,&#10;&#9;&quot;LocalPurchaseData&quot;: {&#10;&#9;&#9;&quot;items&quot;: []&#10;&#9;},&#10;&#9;&quot;GDPREnabled&quot;: true,&#10;&#9;&quot;PrivacyPolicyLink&quot;: &quot;&quot;,&#10;&#9;&quot;TermsOfServiceLink&quot;: &quot;https://www.playtika.com/terms-service&quot;,&#10;&#9;&quot;Campaign4OO7Checked&quot;: false,&#10;&#9;&quot;Campaign2ndDayRetentionChecked&quot;: false,&#10;&#9;&quot;SendLogsAfterComplain&quot;: false,&#10;&#9;&quot;LogsAfterComplainTimestamp&quot;: 0&#10;}";

			_decodedText = WebUtility.HtmlDecode(_encodedText);
			_encodedText = WebUtility.HtmlEncode(_decodedText);
		}

		public static IEnumerable<IEncoder> Encoders
		{
			get
			{
				return new IEncoder[]
				{
					new HtmlEncoder(),
					new GenericEncoder()
				};
			}
		}

		[Test]
		public void EncodeStringOnAndroidButNotOnGeneric_GetEncodeResult()
		{
			var storedDataConverter = _encoder;
			storedDataConverter.Should().NotBeNull();

			var encodeResult = storedDataConverter.Encode(_decodedText);
			var expectedResult = storedDataConverter is HtmlEncoder ? _encodedText : _decodedText;

			encodeResult.Should().Be(expectedResult);
		}

		[Test]
		public void DecodeStringOnAndroidButNotOnGeneric_DecodeCompleted()
		{
			var storedDataConverter = _encoder;
			storedDataConverter.Should().NotBeNull();

			var decodeResult = storedDataConverter.Decode(_encodedText);
			var expectedResult = storedDataConverter is HtmlEncoder ? _decodedText : _encodedText;
			decodeResult.Should().Be(expectedResult);
		}

		[Test]
		public void EncodeNumber_EncodeCompleted()
		{
			var storedDataConverter = _encoder;
			storedDataConverter.Should().NotBeNull();

			var decoded = 50.ToString();
			var encoded = storedDataConverter.Encode(decoded);

			encoded.Should().Be(decoded);
		}

		[Test]
		public void DecodeNumber_DecodeCompleted()
		{
			var storedDataConverter = _encoder;
			storedDataConverter.Should().NotBeNull();

			var encoded = 50.ToString();
			var decoded = storedDataConverter.Decode(encoded);

			decoded.Should().Be(encoded);
		}

		[Test]
		public void DecodeNonEncodedText_DecodeCompleted()
		{
			var storedDataConverter = _encoder;
			storedDataConverter.Should().NotBeNull();

			var decoded = storedDataConverter.Decode(_decodedText);

			decoded.Should().Be(_decodedText);
		}

		[Test]
		public void EncodeDecodeAndAgainEncodeDecode_Completed()
		{
			var storedDataConverter = _encoder;
			storedDataConverter.Should().NotBeNull();

			var encoded = storedDataConverter.Encode(_decodedText);
			var decoded = storedDataConverter.Decode(_decodedText);
			var encoded2 = storedDataConverter.Encode(decoded);
			var decoded2 = storedDataConverter.Decode(encoded2);

			encoded.Should().Be(encoded2);
			decoded.Should().Be(decoded2);
		}
	}
}