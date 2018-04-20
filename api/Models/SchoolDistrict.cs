using System.Collections.Generic;

namespace api.Models
{
	public class SchoolDistrict
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public decimal Rate { get; set; }
		public decimal AlternateRate { get; set; }

		public IList<Student> Students { get; set; }
	}
}
