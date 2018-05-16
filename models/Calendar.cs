using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace models
{
	[Table("CalendarDays")]
	public class CalendarDay
	{
		public int Id { get; set; }
		public string DayOfWeek { get; set; }
		public DateTime Date { get; set; }
		public byte SchoolDay { get; set; }
		public byte Membership { get; set; }

		public virtual Calendar Calendar { get; set; }
	}

	public class Calendar
	{
		public int Id { get; set; }
		public string SchoolYear { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public virtual IList<CalendarDay> Days { get; set; }
	}
}
