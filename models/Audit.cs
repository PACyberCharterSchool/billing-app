using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace models
{
	public static class AuditActivity
	{
		public const string EDIT_STUDENT_RECORD = "EditStudentRecord";
		public const string UPDATE_SCHOOL_CALENDAR = "UpdateSchoolCalendar";
		public const string UPDATE_TEMPLATE = "UpdateTemplate";
	}

	public class AuditHeader
	{
		public int Id { get; set; }
		[Required] public string Username { get; set; }
		[Required] public string Activity { get; set; }
		[Required] public DateTime Timestamp { get; set; }
		[Required] public string Identifier { get; set; }

		public virtual IList<AuditDetail> Details { get; set; }
	}

	[Table("AuditDetails")]
	public class AuditDetail
	{
		public int Id { get; set; }
		[Required] public string Field { get; set; }
		public string Previous { get; set; }
		public string Next { get; set; }

		public virtual AuditHeader Header { get; set; }
	}
}
