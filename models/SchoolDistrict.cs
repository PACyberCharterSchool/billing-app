using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace models
{
	public class SchoolDistrictPaymentType : Enumerable<SchoolDistrictPaymentType>
	{
		private SchoolDistrictPaymentType(string value) : base(value) { }

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

		[JsonConverter(typeof(SchoolDistrictPaymentTypeJsonConverter))]
		public SchoolDistrictPaymentType PaymentType { get; set; }

		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public IList<Student> Students { get; set; }
	}
}
