using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Common;
using api.Controllers;
using api.Tests.Util;
using models;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class CalendarsControllerTests
	{
		private PacBillContext _context;
		private Mock<ICalendarRepository> _calendars;
		private ILogger<CalendarsController> _logger;
		private Mock<IAuditRecordRepository> _audits;
		private CalendarsController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("calendars-controller").Options);
			_calendars = new Mock<ICalendarRepository>();
			_audits = new Mock<IAuditRecordRepository>();
			_logger = new TestLogger<CalendarsController>();

			_uut = new CalendarsController(_context, _calendars.Object, _audits.Object, _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
		}

		public static Calendar NewCalendar(DateTime time) => new Calendar
		{
			SchoolYear = "2017-2018",
			Days = new List<CalendarDay> {
				new CalendarDay{
					DayOfWeek = "Nonceday",
					Date = time.Date,
					SchoolDay = 1,
					Membership = 180,
					},
				},
		};

		public static void AssertCalendar(Calendar actual, Calendar calendar)
		{
			Assert.That(actual.SchoolYear, Is.EqualTo(calendar.SchoolYear));
			Assert.That(actual.Days, Has.Count.EqualTo(calendar.Days.Count));
			for (var i = 0; i < actual.Days.Count; i++)
			{
				Assert.That(actual.Days[i].DayOfWeek, Is.EqualTo(calendar.Days[i].DayOfWeek));
				Assert.That(actual.Days[i].Date, Is.EqualTo(calendar.Days[i].Date));
				Assert.That(actual.Days[i].SchoolDay, Is.EqualTo(calendar.Days[i].SchoolDay));
				Assert.That(actual.Days[i].Membership, Is.EqualTo(calendar.Days[i].Membership));
			}
		}

		[Test]
		public async Task GetGets()
		{
			var calendar = NewCalendar(DateTime.Now);
			_calendars.Setup(cs => cs.Get(calendar.SchoolYear)).Returns(calendar);

			var result = await _uut.Get(calendar.SchoolYear);
			Assert.That(result, Is.TypeOf<ObjectResult>());
			var value = ((ObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<CalendarsController.CalendarResponse>());
			var actual = ((CalendarsController.CalendarResponse)value).Calendar;

			AssertCalendar(actual, calendar);
		}

		[Test]
		public async Task GetReturnsNotFound()
		{
			var year = "2017-2018";
			_calendars.Setup(cs => cs.Get(year)).Throws(new NotFoundException());

			var result = await _uut.Get(year);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		[TestCase("sample-calendar.csv", "text/csv")]
		[TestCase("sample-calendar.xlsx", ContentTypes.XLSX)]
		[TestCase("sample-calendar-empty-first.xlsx", ContentTypes.XLSX)]
		[TestCase("sample-calendar-empty-last.xlsx", ContentTypes.XLSX)]
		public async Task UploadUploads(string fileName, string contentType)
		{
			var formFile = new Mock<IFormFile>();

			var file = File.OpenRead($"../../../TestData/{fileName}");
			formFile.Setup(f => f.ContentType).Returns(contentType).Verifiable();
			formFile.Setup(f => f.OpenReadStream()).Returns(file).Verifiable();

			var year = "2017-2018";
			var day = "Nonceday";
			var date = new DateTime(2017, 8, 30);
			var schoolDay = 1;
			var membership = 180;
			_calendars.Setup(cs => cs.CreateOrUpdate(It.Is<Calendar>(c =>
				c.SchoolYear == year &&
				c.Days.Count == 1 &&
				c.Days[0].DayOfWeek == day &&
				c.Days[0].Date == date &&
				c.Days[0].SchoolDay == schoolDay &&
				c.Days[0].Membership == membership
			))).Returns<Calendar>(c => c).Verifiable();

			var result = await _uut.Upload(year, formFile.Object);
			Assert.That(result, Is.TypeOf<CreatedResult>());
			Assert.That(((CreatedResult)result).Location, Is.EqualTo($"/api/calendars/{year}"));
			var value = ((CreatedResult)result).Value;

			Assert.That(value, Is.TypeOf<CalendarsController.CalendarResponse>());
			var actual = ((CalendarsController.CalendarResponse)value).Calendar;

			Assert.That(actual.SchoolYear, Is.EqualTo(year));
			Assert.That(actual.Days, Has.Count.EqualTo(1));
			Assert.That(actual.Days[0].DayOfWeek, Is.EqualTo(day));
			Assert.That(actual.Days[0].Date, Is.EqualTo(date));
			Assert.That(actual.Days[0].SchoolDay, Is.EqualTo(schoolDay));
			Assert.That(actual.Days[0].Membership, Is.EqualTo(membership));

			formFile.Verify();
			_calendars.Verify();
		}

		[Test]
		public async Task UploadReturnsBadRequestWhenInvalidContentType()
		{
			var formFile = new Mock<IFormFile>();
			var contentType = "bad";
			formFile.Setup(f => f.ContentType).Returns(contentType);

			var result = await _uut.Upload("2017-2018", formFile.Object);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var actual = ((ErrorResponse)value).Error;

			Assert.That(actual, Is.EqualTo($"Invalid file Content-Type '{contentType}'."));
		}
	}
}
