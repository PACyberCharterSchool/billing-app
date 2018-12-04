using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace models
{
	public class Refund
	{
		public int Id { get; set; }
		public double Amount { get; set; }
		public string CheckNumber { get; set; }
		[Column(TypeName = "date")]
		public DateTime Date { get; set; }
		public string SchoolYear { get; set; }
		public string Username { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public virtual SchoolDistrict SchoolDistrict { get; set; }
	}
}
