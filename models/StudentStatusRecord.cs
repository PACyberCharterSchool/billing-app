using System;

namespace models
{
	public class StudentStatusRecord
	{
		public int Id { get; set; }
		public int SchoolDistrictId { get; set; }
		public string SchoolDistrictName { get; set; }
		public int StudentId { get; set; }
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
		public DateTime BatchTime { get; set; }
		public string BatchFilename { get; set; }
		public string BatchHash { get; set; }
	}

	public class PendingStudentStatusRecord : StudentStatusRecord { }

	public class CommittedStudentStatusRecord : StudentStatusRecord
	{
		public DateTime CommitTime { get; set; }
	}
}
