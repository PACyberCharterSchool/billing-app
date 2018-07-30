using System;
using System.ComponentModel.DataAnnotations;

namespace models
{
	public static class AuditRecordActivity
	{
		public const string COMMIT_GENIUS = "CommitGenius";
	}

	public class AuditRecord
	{
		public int Id { get; set; }

		[Required]
		public string Username { get; set; }

		[Required]
		public string Activity { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
