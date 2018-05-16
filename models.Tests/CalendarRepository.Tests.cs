using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using models.Tests.Util;

namespace models
{
	[TestFixture]
	public class CalendarRepositoryTests
	{
		private PacBillContext _context;
		private ILogger<CalendarRepository> _logger;

		private CalendarRepository _uut;

		public PacBillContext NewContext() =>
		 new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
		 	EnableSensitiveDataLogging().
		 	UseInMemoryDatabase("calendar-repository").Options);

		[SetUp]
		public void SetUp()
		{
			_context = NewContext();
			_logger = new TestLogger<CalendarRepository>();

			_uut = new CalendarRepository(_context, _logger);
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
					new CalendarDay
					{
						DayOfWeek = "Monday",
						Date = time.Date.AddDays(-1),
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
		public void CreateOrUpdateCreates()
		{
			var time = DateTime.Now;
			var calendar = NewCalendar(time);

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, calendar));
			Assert.That(result, Is.EqualTo(calendar));

			var actual = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == result.Id);
			AssertCalendar(actual, calendar);
			Assert.That(actual.Created, Is.EqualTo(time));
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void CreateOrUpdateWithSameObjectUpdates()
		{
			var time = DateTime.Now;
			var calendar = NewCalendar(time);
			calendar.Created = time.AddDays(-1);
			_context.Add(calendar);
			_context.SaveChanges();

			calendar.Days[0].DayOfWeek = "Nonceday";

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, calendar));
			AssertCalendar(result, calendar);

			var actual = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == result.Id);
			AssertCalendar(actual, calendar);
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void CreateOrUpdateWithDifferentObjectUpdates()
		{
			var time = DateTime.Now;
			var calendar = NewCalendar(time);
			calendar.Created = time.AddDays(-1);
			using (var ctx = NewContext())
			{
				ctx.Add(calendar);
				ctx.SaveChanges();
			}

			var update = NewCalendar(time);
			update.Id = calendar.Id;
			update.Days[0].Id = calendar.Days[0].Id;
			update.Days[0].DayOfWeek = "Nonceday";

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, update));
			AssertCalendar(result, update);

			var actual = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == result.Id);
			AssertCalendar(actual, update);
			Assert.That(actual.LastUpdated, Is.EqualTo(time));
		}

		[Test]
		public void CreateOrUpdateWithFewerDaysDeletesDays()
		{
			var time = DateTime.Now;
			var calendar = NewCalendar(time);
			calendar.Days.Add(new CalendarDay
			{
				DayOfWeek = "Tuesday",
				Date = time.Date.AddDays(1),
				SchoolDay = 2,
				Membership = 179,
			});
			using (var ctx = NewContext())
			{
				ctx.Add(calendar);
				ctx.SaveChanges();
			}

			var inter = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == calendar.Id);
			Assert.That(inter.Days, Has.Count.EqualTo(2));

			calendar.Days = new List<CalendarDay> {
				new CalendarDay
				{
					DayOfWeek = "Nonceday",
					Date = time.Date.AddDays(-2),
					SchoolDay = 3,
					Membership = 178,
				},
			};

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, calendar));

			var actual = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == result.Id);
			Assert.That(actual.Days, Has.Count.EqualTo(1));
		}

		[Test]
		public void CreateOrUpdateWithMoreDaysCreatesDays()
		{
			var time = DateTime.Now;
			var calendar = NewCalendar(time);
			using (var ctx = NewContext())
			{
				ctx.Add(calendar);
				ctx.SaveChanges();
			}

			var inter = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == calendar.Id);
			Assert.That(inter.Days, Has.Count.EqualTo(1));

			calendar.Days = new List<CalendarDay> {
				new CalendarDay
				{
					DayOfWeek = "Nonceday",
					Date = time.Date.AddDays(-2),
					SchoolDay = 3,
					Membership = 178,
				},
				new CalendarDay
				{
					DayOfWeek = "Twonceday",
					Date = time.Date.AddDays(-3),
					SchoolDay = 4,
					Membership = 179,
				},
			};

			var result = _context.SaveChanges(() => _uut.CreateOrUpdate(time, calendar));

			var actual = NewContext().Calendars.Include(c => c.Days).Single(c => c.Id == result.Id);
			Assert.That(actual.Days, Has.Count.EqualTo(2));
		}

		[Test]
		public void GetGets()
		{
			var calendar = NewCalendar(DateTime.Now);
			using (var ctx = NewContext())
			{
				ctx.Add(calendar);
				ctx.SaveChanges();
			}

			var actual = _uut.Get(calendar.SchoolYear);
			AssertCalendar(actual, calendar);
		}

		[Test]
		public void GetReturnsNotFound()
		{
			Assert.That(() => _uut.Get(""), Throws.TypeOf<NotFoundException>());
		}
	}
}
