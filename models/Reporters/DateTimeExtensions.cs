using System;
using models.Common;

namespace models.Reporters
{
	public static class DateTimeExtensions
	{
		public static bool IsBefore(this DateTime time, Month month, int first, int second)
		{
			DateTime when;
			if (month.FirstYear)
				when = month.AsDateTime(first).EndOfMonth();
			else
				when = month.AsDateTime(second).EndOfMonth();

			return time < when;
		}

		public static bool IsAfter(this DateTime time, Month month, int first, int second) => !time.IsBefore(month, first, second);

		public static DateTime EndOfMonth(this DateTime time)
			=> new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 59, 59);

		public static DateTime LessDays(this DateTime time, int days) => time.Subtract(TimeSpan.FromDays(days));
	}
}
