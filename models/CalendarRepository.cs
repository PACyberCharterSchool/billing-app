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
		Calendar CreateOrUpdate(DateTime time, Calendar update);
		Calendar CreateOrUpdate(Calendar create);
		Calendar Get(string year);
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

		public Calendar CreateOrUpdate(DateTime time, Calendar update)
		{
			var calendar = _calendars.SingleOrDefault(c => c.SchoolYear == update.SchoolYear);
			if (calendar == null)
			{
				update.Created = time;
				update.LastUpdated = time;
				_context.Add(update);
				return update;
			}

			MergeProperties(calendar, update, _excludedCalendarFields);
			calendar.LastUpdated = time;

			if (calendar.Days.Count < update.Days.Count)
				foreach (var day in update.Days.Skip(calendar.Days.Count))
					calendar.Days.Add(day);
			else if (calendar.Days.Count > update.Days.Count)
				calendar.Days = calendar.Days.Take(update.Days.Count).ToList();

			for (var i = 0; i < calendar.Days.Count; i++)
				MergeProperties(calendar.Days[i], update.Days[i], _excludedDaysFields);

			// TODO(Erik): not actually deleting rows, just setting CalendarId to null
			_context.Update(calendar);
			return calendar;
		}

		public Calendar CreateOrUpdate(Calendar update) => CreateOrUpdate(DateTime.Now, update);

		public Calendar Get(string year)
		{
			var calendar = _calendars.SingleOrDefault(c => c.SchoolYear == year);
			if (calendar == null)
				throw new NotFoundException(typeof(Calendar), year);

			return calendar;
		}
	}
}
