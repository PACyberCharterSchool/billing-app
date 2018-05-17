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

		private CalendarsController _uut;

		[SetUp]
		public void SetUp()
		{
			_context = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseInMemoryDatabase("calendars-controller").Options);
			_calendars = new Mock<ICalendarRepository>();
			_logger = new TestLogger<CalendarsController>();

			_uut = new CalendarsController(_context, _calendars.Object, _logger);
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

		// TODO(Erik): test cases?
		[Test]
		public async Task UploadUploads()
		{
			var formFile = new Mock<IFormFile>();

			const string fileName = "sample-calendar.csv";
			using (var file = File.OpenRead($"../../../TestData/{fileName}"))
			{
				formFile.Setup(f => f.ContentType).Returns("text/csv").Verifiable();
				formFile.Setup(f => f.OpenReadStream()).Returns(file).Verifiable();

				_calendars.Setup(cs => cs.CreateOrUpdate(It.IsAny<Calendar>())).Returns<Calendar>(c => c).Verifiable();

				const string year = "2017-2018";
				var result = await _uut.Upload(year, formFile.Object);
				Assert.That(result, Is.TypeOf<CreatedResult>());
				Assert.That(((CreatedResult)result).Location, Is.EqualTo($"/api/calendars/{year}"));
				var value = ((CreatedResult)result).Value;

				Assert.That(value, Is.TypeOf<CalendarsController.CalendarResponse>());
				var actual = ((CalendarsController.CalendarResponse)value).Calendar;

				Assert.That(actual.SchoolYear, Is.EqualTo(year));
				Assert.That(actual.Days, Has.Count.EqualTo(1));
				Assert.That(actual.Days[0].DayOfWeek, Is.EqualTo("Nonceday"));
				Assert.That(actual.Days[0].Date, Is.EqualTo(new DateTime(2017, 8, 30)));
				Assert.That(actual.Days[0].SchoolDay, Is.EqualTo(1));
				Assert.That(actual.Days[0].Membership, Is.EqualTo(180));

				formFile.Verify();
				_calendars.Verify();
			}
		}

		[Test]
		public async Task UploadReturnsBadRequest()
		{
			var formFile = new Mock<IFormFile>();
			var contentType = "bad";
			formFile.Setup(f => f.ContentType).Returns(contentType);

			var result = await _uut.Upload("2017-2018", formFile.Object);
			Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
			var value = ((BadRequestObjectResult)result).Value;

			Assert.That(value, Is.TypeOf<ErrorResponse>());
			var actual = ((ErrorResponse)value).Error;

			Assert.That(actual, Is.EqualTo($"File Content-Type must be text/csv; was {contentType}."));
		}
	}
}
