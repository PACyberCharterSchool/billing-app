using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

using static models.Common.PropertyMerger;

namespace models
{
	public interface ICalendarRepository
	{
		Calendar Get(string year);

		IEnumerable<string> GetYears();
	}

	public class CalendarRepository : ICalendarRepository
	{
		private readonly PacBillContext _context;
		private readonly IIncludableQueryable<Calendar, IList<CalendarDay>> _calendars;
		private readonly ILogger<CalendarRepository> _logger;

		public CalendarRepository(PacBillContext context, ILogger<CalendarRepository> logger)
		{
			_context = context;
			_calendars = context.Calendars.Include(c => c.Days);
			_logger = logger;
		}

		private static IList<string> _excludedCalendarFields = new[] {
			nameof(Calendar.Id),
			nameof(Calendar.Days),
			nameof(Calendar.Created),
			nameof(Calendar.LastUpdated),
		};

		private static IList<string> _excludedDaysFields = new[] {
			nameof(CalendarDay.Id),
			nameof(CalendarDay.Calendar),
		};

		public Calendar Get(string year) => _calendars.SingleOrDefault(c => c.SchoolYear == year);

		public IEnumerable<Calendar> GetMany()
		{
			return _calendars.OrderBy(c => c.SchoolYear);
		}

		public IEnumerable<string> GetYears()
		{
			var calendars = GetMany();
			return calendars.Select(c => c.SchoolYear).Distinct();
		}
	}
}
