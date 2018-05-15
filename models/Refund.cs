using System;

namespace models
{
	public class Refund
	{
		public int Id { get; set; }
		public decimal Amount { get; set; }
		public string CheckNumber { get; set; }
		public DateTime Date { get; set; }
		public string SchoolYear { get; set; }
		public string Username { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public virtual SchoolDistrict SchoolDistrict { get; set; }
	}
}
