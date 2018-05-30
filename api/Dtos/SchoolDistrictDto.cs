using System;

using Newtonsoft.Json;

using models;

namespace api.Dtos
{
	public class SchoolDistrictDto
	{
		public int Id { get; set; }
		public int Aun { get; set; }
		public string Name { get; set; }
		public decimal Rate { get; set; }
		public decimal? AlternateRate { get; set; }
		public decimal SpecialEducationRate { get; set; }

		[JsonConverter(typeof(SchoolDistrictPaymentTypeJsonConverter))]
		public SchoolDistrictPaymentType PaymentType { get; set; }

		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public SchoolDistrictDto(models.SchoolDistrict model)
		{
			this.Id = model.Id;
			this.Aun = model.Aun;
			this.Name = model.Name;
			this.Rate = model.Rate;
			this.AlternateRate = model.AlternateRate;
			this.SpecialEducationRate = model.SpecialEducationRate;
			this.PaymentType = model.PaymentType;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;
		}
	}
}
