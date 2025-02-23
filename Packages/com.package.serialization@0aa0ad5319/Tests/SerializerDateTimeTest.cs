using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Common.Serialization.Implementation;
using FluentAssertions;
using NUnit.Framework;

namespace Common.Serialization.Tests
{
	public class SerializerDateTimeTest
	{
		private CultureInfo _previousCultureInfo;

		private readonly TestClass _testData = new TestClass();

		private static IEnumerable<(CultureInfo, Calendar)> TestCases => new (CultureInfo, Calendar)[]
		{
			(new CultureInfo("ar-SA"), new GregorianCalendar()),
		};

		[Test]
		[TestCaseSource(nameof(TestCases))]
		public void WhenDateTimeSerializedDeserialized_ValuesShouldBeEqual((CultureInfo cultureInfo, Calendar calendar) args)
		{
			_previousCultureInfo = Thread.CurrentThread.CurrentCulture;
			Thread.CurrentThread.CurrentCulture = args.cultureInfo;

			var utcNow = DateTime.UtcNow;
			var calendar = args.calendar;
			_testData.DateTime = new DateTime(calendar.GetYear(utcNow), calendar.GetMonth(utcNow), calendar.GetDayOfMonth(utcNow));

			var serializer = new JsonSerializer(null);

			var serializedString = serializer.Serialize(_testData);
			var deserializedData = serializer.Deserialize<TestClass>(serializedString);

			deserializedData.Should().NotBeNull();

			deserializedData.DateTime.Should().Be(_testData.DateTime);

			Thread.CurrentThread.CurrentCulture = _previousCultureInfo;
		}

		private sealed class TestClass
		{
			public DateTime DateTime;
		}
	}
}