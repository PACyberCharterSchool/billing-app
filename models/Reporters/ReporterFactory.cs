using System;
using System.Reflection;

namespace models.Reporters
{
	public interface IReporterFactory
	{
		IReporter<BulkInvoice, BulkInvoiceReporter.Config> CreateBulkInvoiceReporter(PacBillContext context);
		IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config> CreateBulkStudentInformationReporter(PacBillContext context);
	}

	public class ReporterFactory : IReporterFactory
	{
		public IReporter<BulkInvoice, BulkInvoiceReporter.Config> CreateBulkInvoiceReporter(PacBillContext context)
			=> new BulkInvoiceReporter(context);

		public IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config> CreateBulkStudentInformationReporter(PacBillContext context)
			=> new BulkStudentInformationReporter(context);
	}
}
