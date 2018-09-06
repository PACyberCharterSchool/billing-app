using System;
using System.Reflection;

namespace models.Reporters
{
	public interface IReporterFactory
	{
		IReporter<Invoice, InvoiceReporter.Config> CreateInvoiceReporter(PacBillContext context);
		IReporter<BulkInvoice, BulkInvoiceReporter.Config> CreateBulkInvoiceReporter(PacBillContext context);
		IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config> CreateBulkStudentInformationReporter(PacBillContext context);
	}

	public class ReporterFactory : IReporterFactory
	{
		public IReporter<BulkInvoice, BulkInvoiceReporter.Config> CreateBulkInvoiceReporter(PacBillContext context)
			=> new BulkInvoiceReporter(context);

		public IReporter<Invoice, InvoiceReporter.Config> CreateInvoiceReporter(PacBillContext context)
			=> new InvoiceReporter(context);

		public IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config> CreateBulkStudentInformationReporter(PacBillContext context)
			=> new BulkStudentInformationReporter(context);
	}
}
