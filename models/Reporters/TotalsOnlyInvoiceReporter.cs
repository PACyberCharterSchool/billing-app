using System;
using System.Collections.Generic;
using System.Linq;

using models.Common;

namespace models.Reporters
{
	public class TotalsOnlyEnrollment
	{
		public int Regular { get; set; }
		public int Special { get; set; }
	}

	public class TotalsOnlyEnrollments : Dictionary<string, TotalsOnlyEnrollment> { }

	public class TotalsOnlyTransaction
	{
		public decimal? Sd { get; set; }
		public decimal? Pde { get; set; }
		public decimal? Refund { get; set; }
	}

	public class TotalsOnlyTransactions : Dictionary<string, TotalsOnlyTransaction> { }

	public class TotalsOnlyInvoice
	{
		public string SchoolYear { get; set; }
		public int FirstYear => int.Parse(SchoolYear.Split("-")[0]);
		public int SecondYear => int.Parse(SchoolYear.Split("-")[1]);
		public string Scope { get; set; }
		public DateTime Prepared { get; set; }
		public TotalsOnlyEnrollments Enrollments { get; set; }
		public decimal RegularDue { get; set; }
		public decimal SpecialDue { get; set; }
		public TotalsOnlyTransactions Transactions { get; set; }
		public decimal TotalSd { get; set; }
		public decimal TotalPde { get; set; }
		public decimal TotalRefund { get; set; }
		public decimal TotalPaid { get; set; }
		public decimal NetDue { get; set; }
	}

	public class TotalsOnlyInvoiceReporter : IReporter<TotalsOnlyInvoice, TotalsOnlyInvoiceReporter.Config>
	{
		private readonly PacBillContext _context;

		public TotalsOnlyInvoiceReporter(PacBillContext context) => _context = context;

		public class Config
		{
			public string SchoolYear { get; set; }
			public string Scope { get; set; }
			public IList<int> Auns { get; set; }
			public SchoolDistrictPaymentType PaymentType { get; set; }
		}

		private TotalsOnlyEnrollments GetEnrollments(BulkInvoice bulk)
		{
			var enrollments = new TotalsOnlyEnrollments();
			foreach (var month in Month.AsEnumerable())
				enrollments.Add(month.Name, new TotalsOnlyEnrollment
				{
					Regular = bulk.Districts.Sum(d => EnrollmentsForMonth(d.RegularEnrollments, month)),
					Special = bulk.Districts.Sum(d => EnrollmentsForMonth(d.SpecialEnrollments, month)),
				});

			return enrollments;

			int EnrollmentsForMonth(InvoiceEnrollments ee, Month month)
			{
				return ee.ContainsKey(month.Name) ? ee[month.Name] : 0;
			}
		}

		private TotalsOnlyTransactions GetTransactions(BulkInvoice bulk)
		{
			var transactions = new TotalsOnlyTransactions();
			foreach (var month in Month.AsEnumerable())
			{
				transactions.Add(month.Name, new TotalsOnlyTransaction
				{
					Sd = bulk.Districts.Sum(d => SdForMonth(d.Transactions.AsDictionary(), month).Round()),
					Pde = bulk.Districts.Sum(d => PdeForMonth(d.Transactions.AsDictionary(), month).Round()),
					Refund = bulk.Districts.Sum(d => RefundForMonth(d.Transactions.AsDictionary(), month).Round()),
				});
			}

			return transactions;

			decimal? SdForMonth(IDictionary<string, InvoiceTransaction> tt, Month month)
			{
				return tt.ContainsKey(month.Name) ? tt[month.Name].Payment?.CheckAmount : null;
			}

			decimal? PdeForMonth(IDictionary<string, InvoiceTransaction> tt, Month month)
			{
				return tt.ContainsKey(month.Name) ? tt[month.Name].Payment?.UniPayAmount : null;
			}

			decimal? RefundForMonth(IDictionary<string, InvoiceTransaction> tt, Month month)
			{
				return tt.ContainsKey(month.Name) ? tt[month.Name].Refund : null;
			}
		}

		public TotalsOnlyInvoice GenerateReport(Config config)
		{
			var bulk = new BulkInvoiceReporter(_context).GenerateReport(new BulkInvoiceReporter.Config
			{
				SchoolYear = config.SchoolYear,
				Scope = config.Scope,
				Auns = config.Auns,
				PaymentType = config.PaymentType,
			});

			var report = new TotalsOnlyInvoice
			{
				SchoolYear = config.SchoolYear,
				Scope = config.Scope,
				Prepared = DateTime.Now,
			};

			report.Enrollments = GetEnrollments(bulk);
			report.RegularDue = bulk.Districts.Sum(d => (d.SchoolDistrict.RegularRate * d.RegularEnrollments.Sum(e => e.Value) / 12).Round());
			report.SpecialDue = bulk.Districts.Sum(d => (d.SchoolDistrict.SpecialRate * d.SpecialEnrollments.Sum(e => e.Value) / 12).Round());

			report.Transactions = GetTransactions(bulk);
			report.TotalSd = report.Transactions.Sum(t => t.Value.Sd.HasValue ? t.Value.Sd.Value : 0m).Round();
			report.TotalPde = report.Transactions.Sum(t => t.Value.Pde.HasValue ? t.Value.Pde.Value : 0m).Round();
			report.TotalRefund = report.Transactions.Sum(t => t.Value.Refund.HasValue ? t.Value.Refund.Value : 0m).Round();
			report.TotalPaid = ((report.TotalSd + report.TotalPde) - report.TotalRefund).Round();

			report.NetDue = ((report.RegularDue + report.SpecialDue) - report.TotalPaid).Round();

			return report;
		}
	}
}
