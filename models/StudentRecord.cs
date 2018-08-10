using System;
using System.Collections.Generic;

namespace models
{
	public class StudentRecordsHeader
	{
		public int Id { get; set; }
		public string Scope { get; set; } // 2018.01 or 2018-2019
		public string Filename { get; set; }
		public DateTime Created { get; set; }
		public bool Locked { get; set; }

		public virtual IList<StudentRecord> Records { get; set; }
	}

	public class StudentRecord
	{
		public int Id { get; set; }
		public int SchoolDistrictId { get; set; }
		public string SchoolDistrictName { get; set; }
		public string StudentId { get; set; }
		public string StudentFirstName { get; set; }
		public string StudentMiddleInitial { get; set; }
		public string StudentLastName { get; set; }
		public string StudentGradeLevel { get; set; }
		public DateTime StudentDateOfBirth { get; set; }
		public string StudentStreet1 { get; set; }
		public string StudentStreet2 { get; set; }
		public string StudentCity { get; set; }
		public string StudentState { get; set; }
		public string StudentZipCode { get; set; }
		public string ActivitySchoolYear { get; set; }
		public DateTime StudentEnrollmentDate { get; set; }
		public DateTime? StudentWithdrawalDate { get; set; }
		public bool StudentIsSpecialEducation { get; set; }
		public DateTime? StudentCurrentIep { get; set; }
		public DateTime? StudentFormerIep { get; set; }
		public DateTime? StudentNorep { get; set; }
		public ulong? StudentPaSecuredId { get; set; }
		public DateTime LastUpdated { get; set; }

		public virtual StudentRecordsHeader Header { get; set; }
	}
}
