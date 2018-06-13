using System;
using System.Reflection;

namespace models.Reporters
{
	public interface IReporterFactory
	{
		IReporter<Invoice, InvoiceReporter.Config> CreateInvoiceReporter(PacBillContext context);
	}

	public class ReporterFactory : IReporterFactory
	{
		public IReporter<Invoice, InvoiceReporter.Config> CreateInvoiceReporter(PacBillContext context)
			=> new InvoiceReporter(context);
	}
}
