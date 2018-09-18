using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace models.Common
{
	public class Month
	{
		public string Name { get; set; }
		public int Number { get; set; }
		public bool FirstYear { get; set; }

		private Month(string name, int number, bool firstYear)
		{
			Name = name;
			Number = number;
			FirstYear = firstYear;
		}

		public DateTime AsDateTime(int year, int day) => new DateTime(year, Number, day);
		public DateTime AsDateTime(int year) => AsDateTime(year, 1);

		public readonly static Month July = new Month("July", 7, true);
		public readonly static Month August = new Month("August", 8, true);
		public readonly static Month September = new Month("September", 9, true);
		public readonly static Month October = new Month("October", 10, true);
		public readonly static Month November = new Month("November", 11, true);
		public readonly static Month December = new Month("December", 12, true);
		public readonly static Month January = new Month("January", 1, false);
		public readonly static Month February = new Month("February", 2, false);
		public readonly static Month March = new Month("March", 3, false);
		public readonly static Month April = new Month("April", 4, false);
		public readonly static Month May = new Month("May", 5, false);
		public readonly static Month June = new Month("June", 6, false);

		public static IEnumerable<Month> AsEnumerable() =>
			typeof(Month).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).
				Where(f => f.FieldType == typeof(Month)).
				Select(f => (Month)f.GetValue(null));

		public static IDictionary<string, Month> ByName() => AsEnumerable().ToDictionary(m => m.Name, m => m);
		public static IDictionary<int, Month> ByNumber() => AsEnumerable().ToDictionary(m => m.Number, m => m);
	}
}
