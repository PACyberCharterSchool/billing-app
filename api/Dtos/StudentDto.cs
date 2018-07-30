using System;

using models;

namespace api.Dtos
{
	public class StudentDto
	{
		public int Id { get; set; }
		public string PACyberId { get; set; }
		public ulong? PASecuredId { get; set; }
		public string FirstName { get; set; }
		public string MiddleInitial { get; set; }
		public string LastName { get; set; }
		public string Grade { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public bool IsSpecialEducation { get; set; }
		public DateTime? CurrentIep { get; set; }
		public DateTime? FormerIep { get; set; }
		public DateTime? NorepDate { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public DateTime Created { get; set; }
		public DateTime LastUpdated { get; set; }
		public SchoolDistrictDto SchoolDistrict { get; set; }

		public StudentDto(Student model)
		{
			this.Id = model.Id;
			this.PACyberId = model.PACyberId;
			this.PASecuredId = model.PASecuredId;
			this.FirstName = model.FirstName;
			this.MiddleInitial = model.MiddleInitial;
			this.LastName = model.LastName;
			this.Grade = model.Grade;
			this.DateOfBirth = model.DateOfBirth;
			this.Street1 = model.Street1;
			this.Street2 = model.Street2;
			this.City = model.City;
			this.State = model.State;
			this.ZipCode = model.ZipCode;
			this.IsSpecialEducation = model.IsSpecialEducation;
			this.CurrentIep = model.CurrentIep;
			this.FormerIep = model.FormerIep;
			this.NorepDate = model.NorepDate;
			this.StartDate = model.StartDate;
			this.EndDate = model.EndDate;
			this.Created = model.Created;
			this.LastUpdated = model.LastUpdated;

			if (model.SchoolDistrict != null)
				this.SchoolDistrict = new SchoolDistrictDto(model.SchoolDistrict);
		}
	}
}
