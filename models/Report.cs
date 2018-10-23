using System;

namespace models
{
	public class ReportType : Enumeration<ReportType>
	{
		private ReportType(string value) : base(value) { }

		public static readonly ReportType Invoice = new ReportType("Invoice");
		public static readonly ReportType BulkInvoice = new ReportType("BulkInvoice");
		public static readonly ReportType TotalsOnly = new ReportType("TotalsOnlyInvoice");

		public static readonly ReportType StudentInformation = new ReportType("StudentInformation");
		public static readonly ReportType BulkStudentInformation = new ReportType("BulkStudentInformation");

		public static readonly ReportType AccountsReceivableAging = new ReportType("AccountsReceivableAging");
		public static readonly ReportType AccountsReceivableAsOf = new ReportType("AccountsReceivableAsOf");

		public static readonly ReportType Csiu = new ReportType("Csiu");
		public static readonly ReportType UniPayInvoiceSummary = new ReportType("UniPayInvoiceSummary");
	}

	public class ReportMetadata
	{
		public int Id { get; set; }
		public ReportType Type { get; set; }
		public string SchoolYear { get; set; }
		public string Scope { get; set; }
		public string Name { get; set; }
		public bool Approved { get; set; }
		public DateTime Created { get; set; }
	}

	public class Report : ReportMetadata
	{
		public string Data { get; set; }
		public byte[] Xlsx { get; set; }
		public byte[] Pdf { get; set; }
	}
}
