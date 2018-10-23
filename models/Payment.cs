using System;

namespace models
{
	public class PaymentType : Enumeration<PaymentType>
	{
		private PaymentType(string value) : base(value) { }
		private PaymentType() : base() { }

		public static readonly PaymentType Check = new PaymentType("Check");
		public static readonly PaymentType UniPay = new PaymentType("UniPay");
	}

	public class Payment
	{
		public int Id { get; set; }
		public string PaymentId { get; set; }
		public int Split { get; set; }
		public DateTime Date { get; set; }
		public string ExternalId { get; set; }
		public PaymentType Type { get; set; }
		public double Amount { get; set; }
		public string SchoolYear { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public virtual SchoolDistrict SchoolDistrict { get; set; }
	}
}
