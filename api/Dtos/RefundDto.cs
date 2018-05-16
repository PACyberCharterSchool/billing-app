using System;

using models;

namespace api.Dtos
{
	public class RefundDto
	{
		public int Id { get; set; }
		public decimal Amount { get; set; }
		public string CheckNumber { get; set; }
		public DateTime Date { get; set; }
		public string SchoolYear { get; set; }
		public string Username { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }

		public SchoolDistrictDto SchoolDistrict { get; set; }

		public RefundDto(Refund model)
		{
			this.Id = model.Id;
			this.Amount = model.Amount;
			this.CheckNumber = model.CheckNumber;
			this.Date = model.Date;
			this.SchoolYear = model.SchoolYear;
			this.Username = model.Username;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;
			this.SchoolDistrict = new SchoolDistrictDto(model.SchoolDistrict);
		}
	}
}
