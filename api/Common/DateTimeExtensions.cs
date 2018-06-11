using System;

namespace api.Common
{
	public static class DateTimeExtensions
	{
		private static DateTime _epoch = new DateTime(1970, 1, 1);

		public static long ToSecondsFromEpoch(this DateTime time)
		{
			return (long)(time - _epoch).TotalSeconds;
		}
	}
}
