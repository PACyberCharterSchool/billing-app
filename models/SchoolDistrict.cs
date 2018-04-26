using System;
using System.Collections.Generic;

namespace models
{
	public static class SchoolDistrictPaymentType
	{
		public const string ACH = "ACH";
		public const string CHECK = "Check";
	}

	public class SchoolDistrict
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public decimal Rate { get; set; }
		public decimal? AlternateRate { get; set; }
		public string PaymentType { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public IList<Student> Students { get; set; }
	}
}
