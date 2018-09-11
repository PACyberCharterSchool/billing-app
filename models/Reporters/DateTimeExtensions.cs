using System;

namespace models.Reporters
{
	public static class DateTimeExtensions
	{
		public static bool IsAfter(this DateTime time, int month)
			=> month >= 7 && month >= time.Month || month < 7 && month <= time.Month;

		public static bool IsBefore(this DateTime time, int month) => !time.IsAfter(month);

		public static DateTime EndOfMonth(this DateTime time)
			=> new DateTime(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month));
	}
}
