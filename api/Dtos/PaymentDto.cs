using System;

using Newtonsoft.Json;

using models;

namespace api.Dtos
{
	public class PaymentDto
	{
		public int Id { get; set; }
		public string PaymentId { get; set; }
		public int Split { get; set; }
		public DateTime Date { get; set; }
		public string ExternalId { get; set; }

		[JsonConverter(typeof(PaymentTypeJsonConverter))]
		public PaymentType Type { get; set; }

		public decimal Amount { get; set; }
		public string SchoolYear { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public SchoolDistrictDto SchoolDistrict { get; set; }

		public PaymentDto(Payment model)
		{
			this.Id = model.Id;
			this.PaymentId = model.PaymentId;
			this.Split = model.Split;
			this.Date = model.Date;
			this.ExternalId = model.ExternalId;
			this.Type = model.Type;
			this.Amount = model.Amount;
			this.SchoolYear = model.SchoolYear;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;
			this.SchoolDistrict = new SchoolDistrictDto(model.SchoolDistrict);
		}
	}
}
