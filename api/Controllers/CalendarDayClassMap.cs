using CsvHelper.Configuration;

using models;

namespace api.Controllers
{
	public class CalendarDayClassMap : ClassMap<CalendarDay>
	{
		public CalendarDayClassMap()
		{
			Map(m => m.DayOfWeek).Name("DAY");
			Map(m => m.Date).Name("DATE");
			Map(m => m.SchoolDay).Name("SCHOOL DAY");
			Map(m => m.Membership).Name("MEMBERSHIP");
		}
	}
}
