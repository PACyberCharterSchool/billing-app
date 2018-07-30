using System;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace models
{
	public class StudentActivity : Enumeration<StudentActivity>
	{
		private StudentActivity(string value) : base(value) { }
		private StudentActivity() : base() { }

		public static readonly StudentActivity NewStudent = new StudentActivity("NewStudent");
		public static readonly StudentActivity DateOfBirthChange = new StudentActivity("DateOfBirthChange");
		public static readonly StudentActivity DistrictEnrollment = new StudentActivity("DistrictEnrollment");
		public static readonly StudentActivity DistrictWithdrawal = new StudentActivity("DistrictWithdrawal");
		public static readonly StudentActivity NameChange = new StudentActivity("NameChange");
		public static readonly StudentActivity GradeChange = new StudentActivity("GradeChange");
		public static readonly StudentActivity AddressChange = new StudentActivity("AddressChange");
		public static readonly StudentActivity SpecialEducationEnrollment = new StudentActivity("SpecialEducationEnrollment");
		public static readonly StudentActivity SpecialEducationWithdrawal = new StudentActivity("SpecialEducationWithdrawal");
		public static readonly StudentActivity CurrentIepChange = new StudentActivity("CurrentIepChange");
		public static readonly StudentActivity FormerIepChange = new StudentActivity("FormerIepChange");
		public static readonly StudentActivity NorepChange = new StudentActivity("NorepChange");
		public static readonly StudentActivity PASecuredChange = new StudentActivity("PASecuredChange");
	}

	public class StudentActivityRecord
	{
		public int Id { get; set; }
		public string PACyberId { get; set; }

		[Required]
		[JsonConverter(typeof(StudentActivityJsonConverter))]
		public StudentActivity Activity { get; set; }

		public int Sequence { get; set; }
		public string PreviousData { get; set; }
		public string NextData { get; set; }
		public DateTime Timestamp { get; set; }
		public string BatchHash { get; set; }
	}
}
