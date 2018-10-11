using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace models
{
	public class SchoolDistrictPaymentType : Enumeration<SchoolDistrictPaymentType>
	{
		private SchoolDistrictPaymentType(string value) : base(value) { }
		private SchoolDistrictPaymentType() : base() { }

		public static readonly SchoolDistrictPaymentType Ach = new SchoolDistrictPaymentType("UniPay");
		public static readonly SchoolDistrictPaymentType Check = new SchoolDistrictPaymentType("Check");
	}

	public class SchoolDistrict
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public double Rate { get; set; }
		public double? AlternateRate { get; set; }
		public double SpecialEducationRate { get; set; }
		public double? AlternateSpecialEducationRate { get; set; }
		public SchoolDistrictPaymentType PaymentType { get; set; }

		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
