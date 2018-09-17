using System;
using System.ComponentModel.DataAnnotations;

namespace models
{
	public static class AuditRecordActivity
	{
		public const string EDIT_STUDENT_RECORD = "EditStudentRecord";
	}

	public class AuditRecord
	{
		public int Id { get; set; }

		[Required]
		public string Username { get; set; }

		[Required]
		public string Activity { get; set; }
		public DateTime Timestamp { get; set; }

		public string Identifier { get; set; }
		public string Field { get; set; }
		public string Previous { get; set; }
		public string Next { get; set; }
	}
}
