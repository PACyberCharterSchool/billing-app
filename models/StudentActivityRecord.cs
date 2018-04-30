using System;
using System.ComponentModel.DataAnnotations;

namespace models
{
	public static class StudentActivity
	{
		public const string NEW_STUDENT = "NewStudent";
		public const string DATEOFBIRTH_CHANGE = "DateOfBirthChange";
		public const string DISTRICT_ENROLL = "DistrictEnrollment";
		public const string DISTRICT_WITHDRAW = "DistrictWithdraw";
		public const string NAME_CHANGE = "NameChange";
		public const string GRADE_CHANGE = "GradeChange";
		public const string ADDRESS_CHANGE = "AddressChange";
		public const string SPECIAL_ENROLL = "SpecialEducationEnroll";
		public const string SPECIAL_WITHDRAW = "SpecialEducationWithdraw";
		public const string CURRENTIEP_CHANGE = "CurrentIepChange";
		public const string FORMERIEP_CHANGE = "FormerIepChange";
		public const string NOREP_CHANGE = "NorepChange";
		public const string PASECURED_CHANGE = "PASecuredChange";
	}

	public class StudentActivityRecord
	{
		public int Id { get; set; }
		public int PACyberId { get; set; } // TODO(Erik): string

		[Required]
		public string Activity { get; set; }

		public string PreviousData { get; set; }
		public string NextData { get; set; }
		public DateTime Timestamp { get; set; }
		public string BatchHash { get; set; }
	}
}
