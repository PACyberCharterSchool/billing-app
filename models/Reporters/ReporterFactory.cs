using System;
using System.Reflection;

namespace models.Reporters
{
	public interface IReporterFactory
	{
		IReporter<AccountsReceivableAging, AccountsReceivableAgingReporter.Config> CreateAccountsReceivableAgingReporter(PacBillContext context);
		IReporter<AccountsReceivableAsOf, AccountsReceivableAsOfReporter.Config> CreateAccountsReceivableAsOfReporter(PacBillContext context);
		IReporter<BulkInvoice, BulkInvoiceReporter.Config> CreateBulkInvoiceReporter(PacBillContext context);
		IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config> CreateBulkStudentInformationReporter(PacBillContext context);
		IReporter<CsiuReport, CsiuReporter.Config> CreateCsiuReporter(PacBillContext context);
	}

	public class ReporterFactory : IReporterFactory
	{
		public IReporter<AccountsReceivableAging, AccountsReceivableAgingReporter.Config> CreateAccountsReceivableAgingReporter(PacBillContext context)
			=> new AccountsReceivableAgingReporter(context);

		public IReporter<AccountsReceivableAsOf, AccountsReceivableAsOfReporter.Config> CreateAccountsReceivableAsOfReporter(PacBillContext context)
			=> new AccountsReceivableAsOfReporter(context);

		public IReporter<BulkInvoice, BulkInvoiceReporter.Config> CreateBulkInvoiceReporter(PacBillContext context)
			=> new BulkInvoiceReporter(context);

		public IReporter<BulkStudentInformation, BulkStudentInformationReporter.Config> CreateBulkStudentInformationReporter(PacBillContext context)
			=> new BulkStudentInformationReporter(context);

		public IReporter<CsiuReport, CsiuReporter.Config> CreateCsiuReporter(PacBillContext context)
			=> new CsiuReporter(context);
	}
}
