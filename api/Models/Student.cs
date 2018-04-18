using System;

namespace api.Models
{
	public class Student
	{
		public int Id { get; set; }
		public int PACyberId { get; set; }
		public int PASecuredId { get; set; }
		public string FirstName { get; set; }
		public string MiddleInitial { get; set; }
		public string LastName { get; set; }
		public int Grade { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string ZipCode { get; set; }
		public bool IsSpecialEducation { get; set; }
		public DateTime CurrentIep { get; set; }
		public DateTime FormerIep { get; set; }
		public int SchoolDistrictId { get; set; }
	}
}
