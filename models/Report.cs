using System;

namespace models
{
	public class ReportType : Enumeration<ReportType>
	{
		private ReportType(string value) : base(value) { }

		public static readonly ReportType Invoice = new ReportType("Invoice");
	}

	public class Report
	{
		public int Id { get; set; }
		public ReportType Type { get; set; }
		public string SchoolYear { get; set; }
		public string Name { get; set; }
		public string Data { get; set; }
		public bool Approved { get; set; }
		public DateTime Created { get; set; }
	}
}
