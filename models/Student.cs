using System;
using System.Collections.Generic;
using System.Linq;

namespace models
{
	public class Student
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

		public SchoolDistrict SchoolDistrict { get; set; }

		private static readonly IEnumerable<string> _fields = typeof(Student).GetProperties().Select(p => p.Name);
		public static bool IsValidField(string field)
		{
			return _fields.Select(f => f.ToLower()).Contains(field.ToLower());
		}
	}
}
