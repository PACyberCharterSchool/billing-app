using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace models
{
	public class SchoolDistrictPaymentType : Enumeration<SchoolDistrictPaymentType>
	{
		private SchoolDistrictPaymentType(string value) : base(value) { }
		private SchoolDistrictPaymentType() : base() { }

		public static readonly SchoolDistrictPaymentType Ach = new SchoolDistrictPaymentType("ACH");
		public static readonly SchoolDistrictPaymentType Check = new SchoolDistrictPaymentType("Check");
	}

	public class SchoolDistrict
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public decimal Rate { get; set; }
		public decimal? AlternateRate { get; set; }
		public SchoolDistrictPaymentType PaymentType { get; set; }

		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public virtual IList<Student> Students { get; set; }
	}
}
