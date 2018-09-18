using System;

namespace models.Reporters
{
	public class MissingCalendarException : Exception
	{
		public MissingCalendarException(ReportType type, string schoolYear) :
			base($"Missing required calendar for year '{schoolYear}' when generating {type} report.")
		{ }
	}
}
